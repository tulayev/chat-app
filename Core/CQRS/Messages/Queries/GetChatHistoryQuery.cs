using Core.Helpers;
using Core.Models.DTOs.Message;
using MediatR;

namespace Core.CQRS.Messages.Queries
{
    public record GetChatHistoryQuery(int UserId, int WithUserId) : IRequest<ApiResponse<IEnumerable<MessageDto>>>;
}
