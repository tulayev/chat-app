using Core.Models.DTOs.Auth;
using FluentValidation;

namespace Core.Validators.Auth
{
    public class RegisterUserValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            When(x => x.Avatar != null, () =>
            {
                RuleFor(x => x.Avatar!.ContentType).Must(ct => ct.StartsWith("image/"))
                    .WithMessage("Avatar should be an image.");
                RuleFor(x => x.Avatar!.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("Max allowed size of the avatar is 5MB.");
            });
        }
    }
}
