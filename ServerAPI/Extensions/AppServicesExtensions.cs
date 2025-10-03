using Core.Data;
using Core.Data.Repositories.Message;
using Core.Data.Repositories.User;
using Core.Mappings;
using Core.Models;
using Core.Services.Image;
using Core.Services.JwtToken;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ServerAPI.Extensions
{
    public static class AppServicesExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSignalR();
            // DB (SQLite)
            services.AddDbContext<ChatAppDbContext>(options => options.UseSqlite(config.GetConnectionString("Default")));
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
            // Mapster
            var mapsterConfig = new TypeAdapterConfig();
            mapsterConfig.Scan(typeof(UserMapping).Assembly);
            services.AddSingleton(mapsterConfig);
            services.AddScoped<IMapper, ServiceMapper>();
            // Cloudinary
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            // Repositories
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            // CQRS
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(AppUser).Assembly));
            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(AppUser).Assembly);
            // Custom Services
            services.AddScoped<IImageStoreService, ImageStoreService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
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

            return services;
        }
    }
}
