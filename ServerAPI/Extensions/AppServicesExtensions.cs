using Core.Data;
using Core.Data.Repositories.User;
using Core.Models;
using Core.Services.Image;
using Core.Services.JwtToken;
using FluentValidation;
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
                .AddEntityFrameworkStores<ChatAppDbContext>()
                .AddSignInManager();
            //JWT
            services.Configure<JwtSettings>(config.GetSection("Jwt"));
            var jwt = config.GetSection("Jwt").Get<JwtSettings>()!;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
            // Cloudinary
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            // Repositories
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
                        policy.WithOrigins(
                            "http://localhost:4200",
                            "http://localhost:4201"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            return services;
        }
    }
}
