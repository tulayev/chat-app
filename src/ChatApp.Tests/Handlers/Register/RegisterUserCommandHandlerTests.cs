using ChatApp.Application.Common.Interfaces.Images;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.Register.Commands;
using ChatApp.Application.CQRS.Register.Handlers;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatApp.Tests.Handlers.Register
{
    public class RegisterUserCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = TestDbContextFactory.Create();
        private readonly Mock<IJwtTokenService> _jwtMock = new();
        private readonly Mock<IImageStoreService> _imageStoreMock = new();
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock = new();

        public void Dispose() => _db.Dispose();

        private RegisterUserCommandHandler BuildHandler(out Mock<UserManager<AppUser>> userManagerMock)
        {
            userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            return new RegisterUserCommandHandler(userManagerMock.Object, _jwtMock.Object, _imageStoreMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_UsernameAlreadyTaken_ReturnsFail()
        {
            _db.Users.Add(new AppUser { Id = 1, UserName = "alice", Email = "other@example.com" });
            _db.SaveChanges();
            var handler = BuildHandler(out _);

            var result = await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("alice", "new@example.com", "Password1!", null)),
                CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Username or Email is already taken.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_EmailAlreadyTaken_ReturnsFail()
        {
            _db.Users.Add(new AppUser { Id = 1, UserName = "other", Email = "alice@example.com" });
            _db.SaveChanges();
            var handler = BuildHandler(out _);

            var result = await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("newname", "alice@example.com", "Password1!", null)),
                CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Username or Email is already taken.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_NoAvatar_CreatesUserWithoutCallingImageStoreService()
        {
            var handler = BuildHandler(out var userManagerMock);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("token");

            await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("alice", "alice@example.com", "Password1!", null)),
                CancellationToken.None);

            _imageStoreMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AvatarWithZeroLength_SkipsUpload()
        {
            var handler = BuildHandler(out var userManagerMock);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("token");

            var avatarMock = new Mock<IFormFile>();
            avatarMock.SetupGet(x => x.Length).Returns(0);

            await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("alice", "alice@example.com", "Password1!", avatarMock.Object)),
                CancellationToken.None);

            _imageStoreMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidAvatar_UploadsAndSetsAvatarUrlAndPublicId()
        {
            var handler = BuildHandler(out var userManagerMock);
            AppUser? createdUser = null;
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .Callback<AppUser, string>((u, _) => createdUser = u)
                .ReturnsAsync(IdentityResult.Success);
            _jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("token");

            var avatarMock = new Mock<IFormFile>();
            avatarMock.SetupGet(x => x.Length).Returns(100);
            avatarMock.SetupGet(x => x.FileName).Returns("avatar.png");
            avatarMock.Setup(x => x.OpenReadStream()).Returns(new MemoryStream([1, 2, 3]));

            _imageStoreMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), "avatar.png", null))
                .ReturnsAsync(new AppImageUploadResult("http://img/avatar.png", "pub123"));

            await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("alice", "alice@example.com", "Password1!", avatarMock.Object)),
                CancellationToken.None);

            _imageStoreMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), "avatar.png", null), Times.Once);
            Assert.NotNull(createdUser);
            Assert.Equal("http://img/avatar.png", createdUser!.AvatarUrl);
            Assert.Equal("pub123", createdUser.AvatarPublicId);
        }

        [Fact]
        public async Task Handle_CreateAsyncFails_ReturnsFailWithJoinedIdentityErrors()
        {
            var handler = BuildHandler(out var userManagerMock);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Description = "X" },
                    new IdentityError { Description = "Y" }));

            var result = await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("alice", "alice@example.com", "Password1!", null)),
                CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Registration failed: X; Y", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_TokenServiceReturnsEmpty_ReturnsFail()
        {
            var handler = BuildHandler(out var userManagerMock);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("");

            var result = await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("alice", "alice@example.com", "Password1!", null)),
                CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Something went wrong", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_Success_ReturnsOkWithToken()
        {
            var handler = BuildHandler(out var userManagerMock);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("sample-token");

            var result = await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("alice", "alice@example.com", "Password1!", null)),
                CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("sample-token", result.Data);
        }

        [Fact]
        public async Task Handle_Success_LogsInformation()
        {
            var handler = BuildHandler(out var userManagerMock);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("sample-token");

            await handler.Handle(
                new RegisterUserCommand(new RegisterRequestDto("alice", "alice@example.com", "Password1!", null)),
                CancellationToken.None);

            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
