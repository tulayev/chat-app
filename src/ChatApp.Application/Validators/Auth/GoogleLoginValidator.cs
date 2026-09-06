using ChatApp.Application.CQRS.Login.Commands;
using FluentValidation;

namespace ChatApp.Application.Validators.Auth
{
    public class GoogleLoginValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginValidator()
        {
            RuleFor(x => x.GoogleLoginRequestDto.IdToken).NotEmpty();
        }
    }
}
