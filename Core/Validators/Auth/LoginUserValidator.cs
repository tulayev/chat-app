using Core.CQRS.Login.Queries;
using FluentValidation;

namespace Core.Validators.Auth
{
    public class LoginUserValidator : AbstractValidator<LoginUserQuery>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.LoginRequestDto.UsernameOrEmail)
                .NotEmpty();

            RuleFor(x => x.LoginRequestDto.Password)
                .NotEmpty();
        }
    }
}
