using Core.Helpers;
using MediatR;

namespace Core.CQRS.Message.Queries
{
    public record ChatHistoryQuery(int UserId, int WithUserId) : IRequest<ApiResponse<IEnumerable<Models.Message>>>;
}
