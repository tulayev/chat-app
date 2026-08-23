using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace ChatApp.Infrastructure.Data.Seed
{
    public static class SeedData
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, IHostEnvironment env)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            var filePath = Path.Combine(GetSeedFolder(env), "seed_users.json");
            var userData = await File.ReadAllTextAsync(filePath);
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            if (users == null)
            {
                return;
            }

            foreach (var user in users) 
            {
                await userManager.CreateAsync(user, "Test.123");
            }
        }

        private static string GetSeedFolder(IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                return Path.Combine("../ChatApp.Infrastructure/Data/Seed");
            }
            else
            {
                return Path.Combine("Data/Seed");
            }
        }
    }
}
