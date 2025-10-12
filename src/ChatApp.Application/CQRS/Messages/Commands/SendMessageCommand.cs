using ChatApp.Application.DTOs.Message;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Commands
{
    public class SendMessageCommand : IRequest<SendMessageDto>
    {
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string? Content { get; set; }
    }
}
