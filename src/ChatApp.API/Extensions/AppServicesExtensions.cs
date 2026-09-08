using Asp.Versioning;
using ChatApp.API.Jobs;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;

namespace ChatApp.API.Extensions
{
    public static class AppServicesExtensions
    {
        public const string AuthRateLimiterPolicy = "auth";
        public const string VerificationRateLimiterPolicy = "verification";

        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
        {
            // Rate limiting
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(AuthRateLimiterPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.AddPolicy(VerificationRateLimiterPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("Cors",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            // API versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Swagger
            services.ConfigureOptions<ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                var jwtScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };
                options.AddSecurityDefinition("Bearer", jwtScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });
            });

            // Hangfire
            services.AddHangfire(configuration => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(config.GetConnectionString("Default"))));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddScoped<EmailNotificationForUnreadMessagesJob>();

            return services;
        }
    }
}
