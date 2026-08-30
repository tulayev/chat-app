using ChatApp.Application.CQRS.Users.Handlers;
using ChatApp.Application.CQRS.Users.Queries;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using MapsterMapper;
using Moq;

namespace ChatApp.Tests.Handlers.Users
{
    public class GetAuthenticatedUserQueryHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = TestDbContextFactory.Create();
        private readonly Mock<IMapper> _mapperMock = new();

        public GetAuthenticatedUserQueryHandlerTests()
        {
            _mapperMock.SetupGet(x => x.Config).Returns(MapsterTestConfig.Instance);
        }

        public void Dispose() => _db.Dispose();

        private GetAuthenticatedUserQueryHandler BuildHandler()
        {
            var userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            return new GetAuthenticatedUserQueryHandler(userManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_UserFound_ReturnsOkMappedUserDto()
        {
            _db.Users.Add(new AppUser { Id = 1, UserName = "alice", Email = "alice@example.com" });
            _db.SaveChanges();
            var handler = BuildHandler();

            var result = await handler.Handle(new GetAuthenticatedUserQuery(1), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("alice", result.Data!.Username);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFail()
        {
            var handler = BuildHandler();

            var result = await handler.Handle(new GetAuthenticatedUserQuery(1), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("User is not found!", result.ErrorMessage);
        }
    }
}
