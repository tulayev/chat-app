using ChatApp.Application.Common.Interfaces.Email;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.EmailVerification.Commands;
using ChatApp.Application.CQRS.EmailVerification.Handlers;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatApp.Tests.Handlers.EmailVerification
{
    public class SendVerificationCodeCommandHandlerTests
    {
        private readonly Mock<IVerificationCodeService> _verificationCodeServiceMock = new();
        private readonly Mock<IEmailSenderService> _emailSenderServiceMock = new();
        private readonly Mock<ILogger<SendVerificationCodeCommandHandler>> _loggerMock = new();

        private SendVerificationCodeCommandHandler BuildHandler(out Mock<Microsoft.AspNetCore.Identity.UserManager<AppUser>> userManagerMock)
        {
            userManagerMock = IdentityMockFactory.CreateUserManagerMock();
            return new SendVerificationCodeCommandHandler(
                _verificationCodeServiceMock.Object,
                _emailSenderServiceMock.Object,
                userManagerMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsException()
        {
            var handler = BuildHandler(out var userManagerMock);
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser?)null);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(new SendVerificationCodeCommand("missing@example.com"), CancellationToken.None));

            Assert.Equal("User not found", ex.Message);
        }

        [Fact]
        public async Task Handle_UserFound_GeneratesSixDigitNumericCode()
        {
            var handler = BuildHandler(out var userManagerMock);
            var user = new AppUser { Id = 1, Email = "alice@example.com" };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            string? captured = null;
            _verificationCodeServiceMock.Setup(x => x.StoreCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Callback<string, string, TimeSpan>((_, code, _) => captured = code)
                .Returns(Task.CompletedTask);

            await handler.Handle(new SendVerificationCodeCommand("alice@example.com"), CancellationToken.None);

            Assert.NotNull(captured);
            Assert.Equal(6, captured!.Length);
            var numeric = int.Parse(captured);
            Assert.InRange(numeric, 100000, 999999);
        }

        [Fact]
        public async Task Handle_UserFound_StoresCodeWithTenMinuteTtl()
        {
            var handler = BuildHandler(out var userManagerMock);
            var user = new AppUser { Id = 1, Email = "alice@example.com" };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            await handler.Handle(new SendVerificationCodeCommand("alice@example.com"), CancellationToken.None);

            _verificationCodeServiceMock.Verify(x => x.StoreCodeAsync(
                "alice@example.com", It.IsAny<string>(), TimeSpan.FromMinutes(10)), Times.Once);
        }

        [Fact]
        public async Task Handle_UserFound_SendsEmailContainingTheStoredCode()
        {
            var handler = BuildHandler(out var userManagerMock);
            var user = new AppUser { Id = 1, Email = "alice@example.com" };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            string? captured = null;
            _verificationCodeServiceMock.Setup(x => x.StoreCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Callback<string, string, TimeSpan>((_, code, _) => captured = code)
                .Returns(Task.CompletedTask);

            await handler.Handle(new SendVerificationCodeCommand("alice@example.com"), CancellationToken.None);

            _emailSenderServiceMock.Verify(x => x.SendAsync(
                "alice@example.com", "Email Verification", It.Is<string>(s => s.Contains(captured!))), Times.Once);
        }

        [Fact]
        public async Task Handle_Success_ReturnsOk()
        {
            var handler = BuildHandler(out var userManagerMock);
            var user = new AppUser { Id = 1, Email = "alice@example.com" };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            var result = await handler.Handle(new SendVerificationCodeCommand("alice@example.com"), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("Verification code sent", result.Data);
        }
    }
}
