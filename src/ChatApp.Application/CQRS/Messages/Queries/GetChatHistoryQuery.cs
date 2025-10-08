using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Queries
{
    public record GetChatHistoryQuery(int UserId, int WithUserId) : IRequest<ApiResponse<IEnumerable<MessageDto>>>;
}
