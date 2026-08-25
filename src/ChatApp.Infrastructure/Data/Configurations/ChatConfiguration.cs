using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasOne(x => x.User1)
                .WithMany()
                .HasForeignKey(x => x.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User2)
                .WithMany()
                .HasForeignKey(x => x.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property<int>("MinUserId")
                .HasComputedColumnSql("LEAST(\"User1Id\", \"User2Id\")", stored: true);

            builder.Property<int>("MaxUserId")
                .HasComputedColumnSql("GREATEST(\"User1Id\", \"User2Id\")", stored: true);

            builder.HasIndex("MinUserId", "MaxUserId")
                .IsUnique()
                .HasDatabaseName("IX_Chats_UserPair_Unique");
        }
    }
}
