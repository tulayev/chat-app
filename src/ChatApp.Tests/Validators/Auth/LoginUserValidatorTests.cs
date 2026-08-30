using ChatApp.Application.CQRS.Login.Queries;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Validators.Auth;
using FluentValidation.TestHelper;

namespace ChatApp.Tests.Validators.Auth
{
    public class LoginUserValidatorTests
    {
        private readonly LoginUserValidator _validator = new();

        [Fact]
        public void Should_HaveError_When_UsernameOrEmailIsEmpty()
        {
            var query = new LoginUserQuery(new LoginRequestDto("", "password"));

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.LoginRequestDto.UsernameOrEmail);
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsEmpty()
        {
            var query = new LoginUserQuery(new LoginRequestDto("user", ""));

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.LoginRequestDto.Password);
        }

        [Fact]
        public void Should_NotHaveError_When_RequestIsValid()
        {
            var query = new LoginUserQuery(new LoginRequestDto("user", "password"));

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
