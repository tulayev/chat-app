namespace ChatApp.Application.DTOs.Message
{
    public record SendMessageDto(int Id, int SenderId, int? ReceiverId, string Content, DateTime SentAt);
}
