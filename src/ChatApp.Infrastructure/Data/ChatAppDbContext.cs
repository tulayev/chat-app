using ChatApp.Domain.Abstractions;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChatApp.Infrastructure.Data
{
    public class ChatAppDbContext : IdentityUserContext<AppUser, int>
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ChatAppDbContext(DbContextOptions<ChatAppDbContext> options) : base(options) { }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker
                .Entries<IAuditableEntity>()
                .Where(e => e is IAuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTimeOffset.Now;
                }

                entry.Entity.UpdatedAt = DateTimeOffset.Now;
            }
        }
    }
}
