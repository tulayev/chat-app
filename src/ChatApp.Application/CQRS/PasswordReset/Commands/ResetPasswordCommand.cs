using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.PasswordReset.Commands
{
    public record ResetPasswordCommand(string Email, string Code, string NewPassword) : IRequest<ApiResponse<string>>;
}
