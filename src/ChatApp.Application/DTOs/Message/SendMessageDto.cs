namespace ChatApp.Application.DTOs.Message
{
    public record SendMessageDto(int Id, int SenderId, string Content, DateTime SentAt);
}
