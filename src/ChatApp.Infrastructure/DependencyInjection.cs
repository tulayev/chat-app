using ChatApp.Application.Common.Interfaces.Email;
using ChatApp.Application.Common.Interfaces.Images;
using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services.Email;
using ChatApp.Infrastructure.Services.Images;
using ChatApp.Infrastructure.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SQLitePCL;
using StackExchange.Redis;
using System.Text;

namespace ChatApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) 
        {
            // DB (SQLite)
            Batteries_V2.Init();
            services.AddDbContext<ChatAppDbContext>(options => options.UseSqlite(config.GetConnectionString("Default")));
            // Redis
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect($"{config["Redis:Host"] ?? "localhost"}:{config["Redis:Port"] ?? "6379"}")
            );
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

            return services;
        }
    }
}
