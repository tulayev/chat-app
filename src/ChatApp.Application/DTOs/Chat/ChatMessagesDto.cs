using ChatApp.Application.DTOs.Message;
using ChatApp.Application.DTOs.User;

namespace ChatApp.Application.DTOs.Chat
{
    public record ChatMessagesDto(int ChatId,
        UserDto Contact,
        IReadOnlyCollection<MessageDto> Messages);
}
