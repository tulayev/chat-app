using ChatApp.Application.CQRS.Register.Commands;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Validators.Auth;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ChatApp.Tests.Validators.Auth
{
    public class RegisterUserValidatorTests
    {
        private readonly RegisterUserValidator _validator = new();

        private static RegisterUserCommand BuildCommand(
            string username = "alice",
            string email = "alice@example.com",
            string password = "Password1!",
            IFormFile? avatar = null) =>
            new(new RegisterRequestDto(username, email, password, avatar));

        [Fact]
        public void Should_HaveError_When_UsernameIsEmpty()
        {
            var result = _validator.TestValidate(BuildCommand(username: ""));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Username);
        }

        [Fact]
        public void Should_HaveError_When_UsernameIsShorterThan3Characters()
        {
            var result = _validator.TestValidate(BuildCommand(username: "ab"));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Username);
        }

        [Fact]
        public void Should_HaveError_When_EmailIsEmpty()
        {
            var result = _validator.TestValidate(BuildCommand(email: ""));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Email);
        }

        [Fact]
        public void Should_HaveError_When_EmailIsInvalidFormat()
        {
            var result = _validator.TestValidate(BuildCommand(email: "not-an-email"));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Email);
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsEmpty()
        {
            var result = _validator.TestValidate(BuildCommand(password: ""));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsShorterThan6Characters()
        {
            var result = _validator.TestValidate(BuildCommand(password: "Ab1!"));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordMissingUppercase_WithExpectedMessage()
        {
            var result = _validator.TestValidate(BuildCommand(password: "password1!"));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Password)
                .WithErrorMessage("Password must contain at least one uppercase letter");
        }

        [Fact]
        public void Should_HaveError_When_PasswordMissingDigit_WithExpectedMessage()
        {
            var result = _validator.TestValidate(BuildCommand(password: "Password!"));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Password)
                .WithErrorMessage("Password must contain at least one digit");
        }

        [Fact]
        public void Should_HaveError_When_PasswordMissingSpecialChar_WithExpectedMessage()
        {
            var result = _validator.TestValidate(BuildCommand(password: "Password1"));

            result.ShouldHaveValidationErrorFor(x => x.RegisterRequestDto.Password)
                .WithErrorMessage("Password must contain at least one special character");
        }

        [Fact]
        public void Should_NotHaveError_When_AvatarIsNull()
        {
            var result = _validator.TestValidate(BuildCommand(avatar: null));

            result.ShouldNotHaveValidationErrorFor("RegisterRequestDto.Avatar.ContentType");
        }

        [Fact]
        public void Should_HaveError_When_AvatarContentTypeIsNotImage()
        {
            var avatarMock = new Mock<IFormFile>();
            avatarMock.SetupGet(x => x.ContentType).Returns("application/pdf");
            avatarMock.SetupGet(x => x.Length).Returns(1024);

            var result = _validator.TestValidate(BuildCommand(avatar: avatarMock.Object));

            result.ShouldHaveValidationErrorFor("RegisterRequestDto.Avatar.ContentType");
        }

        [Fact]
        public void Should_HaveError_When_AvatarExceedsFiveMegabytes()
        {
            var avatarMock = new Mock<IFormFile>();
            avatarMock.SetupGet(x => x.ContentType).Returns("image/png");
            avatarMock.SetupGet(x => x.Length).Returns(6 * 1024 * 1024);

            var result = _validator.TestValidate(BuildCommand(avatar: avatarMock.Object));

            result.ShouldHaveValidationErrorFor("RegisterRequestDto.Avatar.Length");
        }

        [Fact]
        public void Should_NotHaveError_When_AvatarIsValidImageUnderSizeLimit()
        {
            var avatarMock = new Mock<IFormFile>();
            avatarMock.SetupGet(x => x.ContentType).Returns("image/png");
            avatarMock.SetupGet(x => x.Length).Returns(1024);

            var result = _validator.TestValidate(BuildCommand(avatar: avatarMock.Object));

            result.ShouldNotHaveValidationErrorFor("RegisterRequestDto.Avatar.ContentType");
            result.ShouldNotHaveValidationErrorFor("RegisterRequestDto.Avatar.Length");
        }

        [Fact]
        public void Should_NotHaveError_When_AllFieldsAreValid()
        {
            var result = _validator.TestValidate(BuildCommand());

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
