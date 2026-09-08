using ChatApp.Application.Common.Interfaces.Email;
using ChatApp.Application.Common.Interfaces.GoogleAuth;
using ChatApp.Application.Common.Interfaces.Images;
using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services.Email;
using ChatApp.Infrastructure.Services.GoogleAuth;
using ChatApp.Infrastructure.Services.Images;
using ChatApp.Infrastructure.Resilience;
using ChatApp.Infrastructure.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.RateLimiting;
using Polly.Retry;
using StackExchange.Redis;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.RateLimiting;

namespace ChatApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) 
        {
            // DB
            services.AddDbContext<ChatAppDbContext>(options => options.UseNpgsql(
                config.GetConnectionString("Default"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null)));
            // Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisHost = config["Redis:Host"] ?? "localhost";
                var redisPort = config["Redis:Port"] ?? "6379";

                var redisOptions = new ConfigurationOptions
                {
                    EndPoints = { $"{redisHost}:{redisPort}" },
                    AbortOnConnectFail = false,
                    ConnectRetry = 3,
                    ConnectTimeout = 5000
                };

                return ConnectionMultiplexer.Connect(redisOptions);
            });
            // Resilience pipelines
            services.AddResiliencePipeline(ResiliencePipelineKeys.RedisVerificationCode, (builder, context) =>
            {
                var logger = context.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("ChatApp.Infrastructure.Resilience");

                builder
                    .AddRetry(new RetryStrategyOptions
                    {
                        ShouldHandle = new PredicateBuilder()
                            .Handle<RedisConnectionException>()
                            .Handle<RedisTimeoutException>(),
                        MaxRetryAttempts = 3,
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        Delay = TimeSpan.FromMilliseconds(200),
                        OnRetry = args =>
                        {
                            logger.LogWarning(args.Outcome.Exception,
                                "Retrying Redis verification-code operation (attempt {AttemptNumber})", args.AttemptNumber + 1);
                            return ValueTask.CompletedTask;
                        }
                    })
                    .AddTimeout(TimeSpan.FromSeconds(2));
            });
            services.AddResiliencePipeline(ResiliencePipelineKeys.SmtpEmail, (builder, context) =>
            {
                var logger = context.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("ChatApp.Infrastructure.Resilience");

                builder
                    .AddRetry(new RetryStrategyOptions
                    {
                        ShouldHandle = new PredicateBuilder()
                            .Handle<SmtpException>()
                            .Handle<SocketException>()
                            .Handle<TimeoutException>(),
                        MaxRetryAttempts = 3,
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        Delay = TimeSpan.FromSeconds(1),
                        OnRetry = args =>
                        {
                            logger.LogWarning(args.Outcome.Exception,
                                "Retrying SMTP email send (attempt {AttemptNumber})", args.AttemptNumber + 1);
                            return ValueTask.CompletedTask;
                        }
                    })
                    .AddTimeout(TimeSpan.FromSeconds(10));
            });
            services.AddResiliencePipeline(ResiliencePipelineKeys.UnreadMessagesEmailThrottle, builder =>
            {
                builder.AddRateLimiter(new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 1,
                    TokensPerPeriod = 1,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                    AutoReplenishment = true,
                    QueueLimit = int.MaxValue,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }));
            });
            // Identity Core
            services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
            })
                .AddSignInManager()
                .AddEntityFrameworkStores<ChatAppDbContext>();
            //JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtTokenKey"]!)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrWhiteSpace(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            // Cloudinary
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Custom Services
            services.AddScoped<IImageStoreService, ImageStoreService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IVerificationCodeService, VerificationCodeService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();

            return services;
        }
    }
}
