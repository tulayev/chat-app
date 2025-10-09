using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.Login.Queries
{
    public record LoginUserQuery(LoginRequestDto LoginRequestDto) : IRequest<ApiResponse<AuthUserDto>>;
}
