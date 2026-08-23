using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Queries
{
    public record GetChatMessagesQuery(int CurrentUserId, int UserId) : IRequest<ApiResponse<IReadOnlyCollection<MessageDto>>>;
}
