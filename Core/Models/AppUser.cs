using Microsoft.AspNetCore.Identity;

namespace Core.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string? AvatarUrl { get; set; }
        public string? AvatarPublicId { get; set; }
    }
}
