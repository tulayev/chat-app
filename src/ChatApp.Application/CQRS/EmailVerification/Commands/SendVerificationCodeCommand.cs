using ChatApp.Application.Helpers;
using MediatR;

namespace ChatApp.Application.CQRS.EmailVerification.Commands
{
    public record SendVerificationCodeCommand(string Email) : IRequest<ApiResponse<string>>;
}
