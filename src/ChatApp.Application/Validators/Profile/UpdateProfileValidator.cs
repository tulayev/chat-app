using ChatApp.Application.CQRS.Profile.Commands;
using FluentValidation;

namespace ChatApp.Application.Validators.Profile
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.Request.Username)
                .NotEmpty()
                .MinimumLength(3);

            When(x => x.Request.Avatar != null, () =>
            {
                RuleFor(x => x.Request.Avatar!.ContentType).Must(ct => ct.StartsWith("image/"))
                    .WithMessage("Avatar should be an image.");
                RuleFor(x => x.Request.Avatar!.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("Max allowed size of the avatar is 5MB.");
            });
        }
    }
}
