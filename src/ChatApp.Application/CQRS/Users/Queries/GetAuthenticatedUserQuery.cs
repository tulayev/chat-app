using ChatApp.Application.DTOs.User;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Users.Queries
{
    public record GetAuthenticatedUserQuery(int CurrentUserId) : IRequest<ApiResponse<UserDto>>;
}
