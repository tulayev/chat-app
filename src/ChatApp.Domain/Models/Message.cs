using ChatApp.Domain.Abstractions;

namespace ChatApp.Domain.Models
{
    public class Message : IAuditableEntity
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public Chat Chat { get; set; } = null!;
        public AppUser Sender { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
