namespace ChatApp.Application.DTOs.Message
{
    public class ChatContactDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? LastMessage { get; set; }
        public DateTime LastMessageDate { get; set; }
    }
}
