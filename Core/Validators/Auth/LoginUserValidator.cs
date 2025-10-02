using Core.Models.DTOs.Auth;
using FluentValidation;

namespace Core.Validators.Auth
{
    public class LoginUserValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.UserNameOrEmail).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
