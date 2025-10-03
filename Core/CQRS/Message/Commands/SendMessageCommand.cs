using Core.Models.DTOs.Message;
using MediatR;

namespace Core.CQRS.Message.Commands
{
    public record SendMessageCommand(int SenderId, int ReceiverId, string Content) : IRequest<MessageDto>;
}
