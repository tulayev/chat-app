using ChatApp.Application.DTOs.User;

namespace ChatApp.Application.DTOs.Message
{
    public record ChatMessageDto(int Id, 
        int ChatId,
        UserDto Sender,
        string Content, 
        DateTime SentAt);
}
