using ChatApp.Application.DTOs.User;

namespace ChatApp.Application.DTOs.Chat
{
    public record ChatDto(int ChatId, 
        UserDto Contact,
        string? LastMessage, 
        DateTime? LastMessageTime);
}
