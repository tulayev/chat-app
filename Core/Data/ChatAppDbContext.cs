using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public class ChatAppDbContext : IdentityUserContext<AppUser, int>
    {
        public ChatAppDbContext(DbContextOptions<ChatAppDbContext> options) : base(options) { }
    }
}
