using ChatApp.Domain.Abstractions;

namespace ChatApp.Domain.Models
{
    public class Chat : IAuditableEntity
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public AppUser User1 { get; set; } = null!;
        public AppUser User2 { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
