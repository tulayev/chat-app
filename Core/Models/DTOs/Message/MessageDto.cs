using Core.Models.DTOs.User;

namespace Core.Models.DTOs.Message
{
    public record MessageDto(int Id, string Content, DateTime SentTime, UserDto From, UserDto To);
}
