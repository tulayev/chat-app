namespace Core.Models.DTOs.Message
{
    public record MessageDto(int Id, int SenderId, int? ReceiverId, string Content, DateTimeOffset SentAt);
}
