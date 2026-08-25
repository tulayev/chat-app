using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasIndex(x => x.SentAt)
                .IsDescending()
                .HasDatabaseName("IX_Messages_SentAt_DESC");
        }
    }
}
