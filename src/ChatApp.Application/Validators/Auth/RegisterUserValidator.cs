using ChatApp.Application.CQRS.Register.Commands;
using FluentValidation;

namespace ChatApp.Application.Validators.Auth
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.RegisterRequestDto.Username)
                .NotEmpty()
                .MinimumLength(3);

            RuleFor(x => x.RegisterRequestDto.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.RegisterRequestDto.Password)
                .NotEmpty()
                .MinimumLength(6)
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"\d").WithMessage("Password must contain at least one digit")
                .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Password must contain at least one special character");

            When(x => x.RegisterRequestDto.Avatar != null, () =>
            {
                RuleFor(x => x.RegisterRequestDto.Avatar!.ContentType).Must(ct => ct.StartsWith("image/"))
                    .WithMessage("Avatar should be an image.");
                RuleFor(x => x.RegisterRequestDto.Avatar!.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("Max allowed size of the avatar is 5MB.");
            });
        }
    }
}
