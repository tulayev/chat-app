using Core.Models.DTOs.Auth;
using MediatR;

namespace Core.CQRS.Login.Queries
{
    public record LoginUserQuery(LoginRequestDto LoginRequestDto) : IRequest<AuthResponse>;
}
