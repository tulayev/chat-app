using ChatApp.Application.DTOs.Profile;
using ChatApp.Application.DTOs.User;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Profile.Commands
{
    public record UpdateProfileCommand(int UserId, UpdateProfileRequestDto Request) : IRequest<ApiResponse<UserDto>>;
}
