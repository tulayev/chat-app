using Core.Data;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace ServerAPI.Extensions
{
    public static class AppServicesExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSignalR();
            // DB
            services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ChatAppDbContext>()
                .AddSignInManager();
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
