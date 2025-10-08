using ChatApp.Application.CQRS.Login.Queries;
using FluentValidation;

namespace ChatApp.Application.Validators.Auth
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
