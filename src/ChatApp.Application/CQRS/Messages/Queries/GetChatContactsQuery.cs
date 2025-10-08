using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Queries
{
    public record GetChatContactsQuery(int CurrentUserId) : IRequest<ApiResponse<IEnumerable<ChatContactDto>>>;
}
