using MediatR;

namespace ChatApp.Application.CQRS.EmailVerification.Commands
{
    public record VerifyEmailCommand(string Email, string Code) : IRequest;
}
