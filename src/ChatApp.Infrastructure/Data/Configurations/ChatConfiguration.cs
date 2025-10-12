using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User1)
                .WithMany()
                .HasForeignKey(x => x.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User2)
                .WithMany()
                .HasForeignKey(x => x.User2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
