using ChatApp.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Extensions
{
    public static class AuditableEntityConfigurationExtensions
    {
        public static void ConfigureAuditableEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IAuditableEntity
        {
            builder.Property<DateTime>("CreatedAt")
                .IsRequired();

            builder.Property<DateTime?>("UpdatedAt");
        }
    }
}
