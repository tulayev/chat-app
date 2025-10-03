namespace Core.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.Now;
        public int SenderId { get; set; }
        public AppUser Sender { get; set; } = null!;
        public int? ReceiverId { get; set; }
        public AppUser? Receiver { get; set; }
    }
}
