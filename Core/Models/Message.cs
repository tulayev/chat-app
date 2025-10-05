using Core.Models.Abstractions;

namespace Core.Models
{
    public class Message : IAuditableEntity
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.Now;
        public int SenderId { get; set; }
        public AppUser Sender { get; set; } = null!;
        public int? ReceiverId { get; set; }
        public AppUser? Receiver { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
