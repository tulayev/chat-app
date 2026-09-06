using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Commands
{
    public record MarkMessagesReadCommand(int ChatId, int ReaderId) : IRequest<ApiResponse<Unit>>;
}
