using ChatApp.Application.DTOs.User;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Users.Queries
{
    public record GetUsersQuery(int CurrentUserId) : IRequest<ApiResponse<IReadOnlyCollection<UserDto>>>;
}
