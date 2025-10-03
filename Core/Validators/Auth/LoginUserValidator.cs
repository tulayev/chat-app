using Core.Models.DTOs.Auth;
using FluentValidation;

namespace Core.Validators.Auth
{
    public class LoginUserValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.UsernameOrEmail).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
