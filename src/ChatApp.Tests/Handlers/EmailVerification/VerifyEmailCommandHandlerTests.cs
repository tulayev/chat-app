using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.EmailVerification.Commands;
using ChatApp.Application.CQRS.EmailVerification.Handlers;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ChatApp.Tests.Handlers.EmailVerification
{
    public class VerifyEmailCommandHandlerTests
    {
        private readonly Mock<IVerificationCodeService> _verificationCodeServiceMock = new();

        private VerifyEmailCommandHandler BuildHandler(out Mock<UserManager<AppUser>> userManagerMock)
        {
            userManagerMock = IdentityMockFactory.CreateUserManagerMock();
            return new VerifyEmailCommandHandler(_verificationCodeServiceMock.Object, userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_NoStoredCode_ThrowsInvalidOperationException()
        {
            var handler = BuildHandler(out _);
            _verificationCodeServiceMock.Setup(x => x.GetCodeAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new VerifyEmailCommand("alice@example.com", "123456"), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CodeMismatch_ThrowsException()
        {
            var handler = BuildHandler(out _);
            _verificationCodeServiceMock.Setup(x => x.GetCodeAsync(It.IsAny<string>())).ReturnsAsync("111111");

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(new VerifyEmailCommand("alice@example.com", "222222"), CancellationToken.None));

            Assert.Equal("Invalid verification code", ex.Message);
        }

        [Fact]
        public async Task Handle_UserNotFoundAfterCodeMatches_ThrowsException()
        {
            var handler = BuildHandler(out var userManagerMock);
            _verificationCodeServiceMock.Setup(x => x.GetCodeAsync(It.IsAny<string>())).ReturnsAsync("111111");
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser?)null);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(new VerifyEmailCommand("alice@example.com", "111111"), CancellationToken.None));

            Assert.Equal("User not found", ex.Message);
        }

        [Fact]
        public async Task Handle_Success_SetsEmailConfirmedTrueBeforeUpdate()
        {
            var handler = BuildHandler(out var userManagerMock);
            var user = new AppUser { Id = 1, Email = "alice@example.com", EmailConfirmed = false };
            _verificationCodeServiceMock.Setup(x => x.GetCodeAsync(It.IsAny<string>())).ReturnsAsync("111111");
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            AppUser? updatedUser = null;
            userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
                .Callback<AppUser>(u => updatedUser = u)
                .ReturnsAsync(IdentityResult.Success);

            await handler.Handle(new VerifyEmailCommand("alice@example.com", "111111"), CancellationToken.None);

            Assert.NotNull(updatedUser);
            Assert.True(updatedUser!.EmailConfirmed);
        }

        [Fact]
        public async Task Handle_Success_DeletesStoredCode()
        {
            var handler = BuildHandler(out var userManagerMock);
            var user = new AppUser { Id = 1, Email = "alice@example.com" };
            _verificationCodeServiceMock.Setup(x => x.GetCodeAsync(It.IsAny<string>())).ReturnsAsync("111111");
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<AppUser>())).ReturnsAsync(IdentityResult.Success);

            await handler.Handle(new VerifyEmailCommand("alice@example.com", "111111"), CancellationToken.None);

            _verificationCodeServiceMock.Verify(x => x.DeleteCodeAsync("alice@example.com"), Times.Once);
        }

        [Fact]
        public async Task Handle_Success_ReturnsOk()
        {
            var handler = BuildHandler(out var userManagerMock);
            var user = new AppUser { Id = 1, Email = "alice@example.com" };
            _verificationCodeServiceMock.Setup(x => x.GetCodeAsync(It.IsAny<string>())).ReturnsAsync("111111");
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<AppUser>())).ReturnsAsync(IdentityResult.Success);

            var result = await handler.Handle(new VerifyEmailCommand("alice@example.com", "111111"), CancellationToken.None);

            Assert.Equal("Email verified", result.Data);
        }
    }
}
