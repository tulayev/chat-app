using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.Property(x => x.UserName)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(x => x.NormalizedUserName)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(x => x.Email)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(x => x.NormalizedEmail)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.EmailConfirmed)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
