using Core.Helpers;
using Core.Models.DTOs.Auth;
using MediatR;

namespace Core.CQRS.Register.Commands
{
    public record RegisterUserCommand(RegisterRequestDto RegisterRequestDto) : IRequest<ApiResponse<AuthResponseDto>>;
}
