﻿using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ChatApp.Infrastructure.Data.Seed
{
    public static class SeedData
    {
        private const string SeedFolder = "../ChatApp.Infrastructure/Data/Seed";

        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            var filePath = $"{SeedFolder}/seed_users.json";
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

        public static async Task SeedChats(ChatAppDbContext db)
        {
            if (await db.Chats.AnyAsync())
            {
                return;
            }

            var filePath = $"{SeedFolder}/seed_chats.json";
            var chatData = await File.ReadAllTextAsync(filePath);
            var chats = JsonSerializer.Deserialize<List<Chat>>(chatData);

            if (chats == null)
            {
                return;
            }

            await db.Chats.AddRangeAsync(chats);

            if (db.ChangeTracker.HasChanges())
            {
                await db.SaveChangesAsync();
            }
        }

        public static async Task SeedMessages(ChatAppDbContext db)
        {
            if (await db.Messages.AnyAsync())
            {
                return;
            }

            var filePath = $"{SeedFolder}/seed_messages.json";
            var messageData = await File.ReadAllTextAsync(filePath);
            var messages = JsonSerializer.Deserialize<List<Message>>(messageData);

            if (messages == null)
            {
                return;
            }

            await db.Messages.AddRangeAsync(messages);

            if (db.ChangeTracker.HasChanges())
            {
                await db.SaveChangesAsync();
            }
        }
    }
}
