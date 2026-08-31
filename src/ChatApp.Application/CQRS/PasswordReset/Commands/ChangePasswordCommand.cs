using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.PasswordReset.Commands
{
    public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword) : IRequest<ApiResponse<string>>;
}
