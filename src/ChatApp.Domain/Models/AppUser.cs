using ChatApp.Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Models
{
    public class AppUser : IdentityUser<int>, IAuditableEntity
    {
        public string? AvatarUrl { get; set; }
        public string? AvatarPublicId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
