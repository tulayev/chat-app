using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Login.Commands
{
    public record GoogleLoginCommand(GoogleLoginRequestDto GoogleLoginRequestDto) : IRequest<ApiResponse<string>>;
}
