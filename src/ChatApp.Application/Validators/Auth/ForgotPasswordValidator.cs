using ChatApp.Application.CQRS.PasswordReset.Commands;
using FluentValidation;

namespace ChatApp.Application.Validators.Auth
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
