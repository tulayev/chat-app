using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var db = services.GetRequiredService<ChatAppDbContext>();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var env = services.GetRequiredService<IHostEnvironment>();
                    await db.Database.MigrateAsync();
                    // Seed data
                    await SeedData.SeedUsers(userManager, env);
                    await SeedData.SeedChats(db, env);
                    await SeedData.SeedMessages(db, env);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            return app;
        }
    }
}
