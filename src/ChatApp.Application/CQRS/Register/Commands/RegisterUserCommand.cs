using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Register.Commands
{
    public record RegisterUserCommand(RegisterRequestDto RegisterRequestDto) : IRequest<ApiResponse<AuthUserDto>>;
}
