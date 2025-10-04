namespace Core.Models.DTOs.Message
{
    public record SendMessageDto(int Id, int SenderId, int? ReceiverId, string Content, DateTimeOffset SentAt);
}
