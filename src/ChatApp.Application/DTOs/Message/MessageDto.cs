using ChatApp.Application.DTOs.User;

namespace ChatApp.Application.DTOs.Message
{
    public record MessageDto(int Id, string Content, DateTime SentAt, UserDto From, UserDto To);
}
