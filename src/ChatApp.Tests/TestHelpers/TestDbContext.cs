using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Tests.TestHelpers
{
    public class TestDbContext : DbContext
    {
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<Message> Messages => Set<Message>();

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>();

            modelBuilder.Entity<Chat>(b =>
            {
                b.HasOne(x => x.User1).WithMany().HasForeignKey(x => x.User1Id).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(x => x.User2).WithMany().HasForeignKey(x => x.User2Id).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Message>();
        }
    }
}
