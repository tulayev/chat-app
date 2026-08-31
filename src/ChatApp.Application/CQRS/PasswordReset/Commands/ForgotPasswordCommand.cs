using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.PasswordReset.Commands
{
    public record ForgotPasswordCommand(string Email) : IRequest<ApiResponse<string>>;
}
