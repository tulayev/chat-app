using ChatApp.Application.DTOs.Message;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Commands
{
    public record SendMessageCommand(int SenderId, int ReceiverId, string Content) : IRequest<SendMessageDto>;
}
