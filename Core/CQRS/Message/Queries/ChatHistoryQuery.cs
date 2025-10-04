using Core.Helpers;
using Core.Models.DTOs.Message;
using MediatR;

namespace Core.CQRS.Message.Queries
{
    public record ChatHistoryQuery(int UserId, int WithUserId) : IRequest<ApiResponse<IEnumerable<MessageDto>>>;
}
