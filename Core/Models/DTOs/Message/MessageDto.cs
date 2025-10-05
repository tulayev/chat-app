using Core.Models.DTOs.User;

namespace Core.Models.DTOs.Message
{
    public record MessageDto(int Id, string Content, DateTime SentAt, UserDto From, UserDto To);
}
