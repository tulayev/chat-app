using Core.Models.DTOs.Message;
using MediatR;

namespace Core.CQRS.Messages.Commands
{
    public record SendMessageCommand(int SenderId, int ReceiverId, string Content) : IRequest<SendMessageDto>;
}
