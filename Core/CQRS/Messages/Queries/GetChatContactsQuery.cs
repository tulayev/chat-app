using Core.Helpers;
using Core.Models.DTOs.Message;
using MediatR;

namespace Core.CQRS.Messages.Queries
{
    public record GetChatContactsQuery(int CurrentUserId) : IRequest<ApiResponse<IEnumerable<ChatContactDto>>>;
}
