using Core.Models.DTOs.Auth;
using MediatR;

namespace Core.CQRS.Register.Commands
{
    public record RegisterUserCommand(RegisterWithAvatarRequestDto RegisterRequestDto) : IRequest<AuthResponse>;
}
