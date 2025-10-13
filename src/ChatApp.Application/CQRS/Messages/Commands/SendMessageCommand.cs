using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Commands
{
    public class SendMessageCommand : IRequest<ApiResponse<Unit>>
    {
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string? Content { get; set; }
    }
}
