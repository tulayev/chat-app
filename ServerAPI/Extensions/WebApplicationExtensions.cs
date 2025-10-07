using Core.Data;
using Core.Data.Seed;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ServerAPI.Extensions
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
                    await db.Database.MigrateAsync();
                    // Seed data
                    await SeedData.SeedUsers(userManager);
                    await SeedData.SeedMessages(db);
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
