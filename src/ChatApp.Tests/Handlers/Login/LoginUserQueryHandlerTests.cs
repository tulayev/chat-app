using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.Login.Handlers;
using ChatApp.Application.CQRS.Login.Queries;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ChatApp.Tests.Handlers.Login
{
    public class LoginUserQueryHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = TestDbContextFactory.Create();

        public void Dispose() => _db.Dispose();

        private static AppUser BuildUser(int id, string username, string email) => new()
        {
            Id = id,
            UserName = username,
            NormalizedUserName = username.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant()
        };

        private LoginUserQueryHandler BuildHandler(
            Mock<SignInManager<AppUser>> signInManagerMock,
            Mock<IJwtTokenService> jwtMock)
        {
            var uow = TestUnitOfWorkFactory.Create(_db);
            return new LoginUserQueryHandler(signInManagerMock.Object, uow.Object, jwtMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFail()
        {
            var userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            var signInManagerMock = IdentityMockFactory.CreateSignInManagerMock(userManagerMock);
            var jwtMock = new Mock<IJwtTokenService>();
            var handler = BuildHandler(signInManagerMock, jwtMock);

            var result = await handler.Handle(new LoginUserQuery(new LoginRequestDto("missing", "password")), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("User is not found", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_MatchesByEmail_FindsUser()
        {
            var user = BuildUser(1, "alice", "alice@example.com");
            _db.Users.Add(user);
            _db.SaveChanges();

            var userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            var signInManagerMock = IdentityMockFactory.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);
            var jwtMock = new Mock<IJwtTokenService>();
            jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("token");
            var handler = BuildHandler(signInManagerMock, jwtMock);

            var result = await handler.Handle(new LoginUserQuery(new LoginRequestDto("alice@example.com", "pw")), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("token", result.Data);
        }

        [Fact]
        public async Task Handle_MatchesByUserName_FindsUser()
        {
            var user = BuildUser(1, "alice", "alice@example.com");
            _db.Users.Add(user);
            _db.SaveChanges();

            var userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            var signInManagerMock = IdentityMockFactory.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);
            var jwtMock = new Mock<IJwtTokenService>();
            jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("token");
            var handler = BuildHandler(signInManagerMock, jwtMock);

            var result = await handler.Handle(new LoginUserQuery(new LoginRequestDto("alice", "pw")), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("token", result.Data);
        }

        [Fact]
        public async Task Handle_IncorrectPassword_ReturnsFail()
        {
            var user = BuildUser(1, "alice", "alice@example.com");
            _db.Users.Add(user);
            _db.SaveChanges();

            var userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            var signInManagerMock = IdentityMockFactory.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Failed);
            var jwtMock = new Mock<IJwtTokenService>();
            var handler = BuildHandler(signInManagerMock, jwtMock);

            var result = await handler.Handle(new LoginUserQuery(new LoginRequestDto("alice", "wrong")), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Login or password is incorrect.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_TokenServiceReturnsEmpty_ReturnsFail()
        {
            var user = BuildUser(1, "alice", "alice@example.com");
            _db.Users.Add(user);
            _db.SaveChanges();

            var userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            var signInManagerMock = IdentityMockFactory.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);
            var jwtMock = new Mock<IJwtTokenService>();
            jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("   ");
            var handler = BuildHandler(signInManagerMock, jwtMock);

            var result = await handler.Handle(new LoginUserQuery(new LoginRequestDto("alice", "pw")), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Something went wrong", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsOkWithToken()
        {
            var user = BuildUser(1, "alice", "alice@example.com");
            _db.Users.Add(user);
            _db.SaveChanges();

            var userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            var signInManagerMock = IdentityMockFactory.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);
            var jwtMock = new Mock<IJwtTokenService>();
            jwtMock.Setup(x => x.CreateToken(It.IsAny<AppUser>())).Returns("sample-token");
            var handler = BuildHandler(signInManagerMock, jwtMock);

            var result = await handler.Handle(new LoginUserQuery(new LoginRequestDto("alice", "pw")), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("sample-token", result.Data);
        }
    }
}
