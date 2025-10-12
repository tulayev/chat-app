using ChatApp.Application.DTOs.Chat;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Queries
{
    public record GetUserChatsQuery(int CurrentUserId) : IRequest<ApiResponse<IEnumerable<ChatDto>>>;
}
