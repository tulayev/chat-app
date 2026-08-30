using ChatApp.Application.CQRS.Users.Handlers;
using ChatApp.Application.CQRS.Users.Queries;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using MapsterMapper;
using Moq;

namespace ChatApp.Tests.Handlers.Users
{
    public class GetUsersQueryHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = TestDbContextFactory.Create();
        private readonly Mock<IMapper> _mapperMock = new();

        public GetUsersQueryHandlerTests()
        {
            _mapperMock.SetupGet(x => x.Config).Returns(MapsterTestConfig.Instance);
        }

        public void Dispose() => _db.Dispose();

        private GetUsersQueryHandler BuildHandler()
        {
            var userManagerMock = IdentityMockFactory.CreateUserManagerMock(_db.Users);
            return new GetUsersQueryHandler(userManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ExcludesCurrentUserFromResults()
        {
            _db.Users.Add(new AppUser { Id = 1, UserName = "self", NormalizedUserName = "SELF", Email = "self@example.com", NormalizedEmail = "SELF@EXAMPLE.COM", EmailConfirmed = true });
            _db.Users.Add(new AppUser { Id = 2, UserName = "other", NormalizedUserName = "OTHER", Email = "other@example.com", NormalizedEmail = "OTHER@EXAMPLE.COM", EmailConfirmed = true });
            _db.SaveChanges();
            var handler = BuildHandler();

            var result = await handler.Handle(new GetUsersQuery(1, null), CancellationToken.None);

            var user = Assert.Single(result.Data!);
            Assert.Equal(2, user.Id);
        }

        [Fact]
        public async Task Handle_ExcludesUsersWithUnconfirmedEmail()
        {
            _db.Users.Add(new AppUser { Id = 2, UserName = "confirmed", NormalizedUserName = "CONFIRMED", Email = "c@example.com", NormalizedEmail = "C@EXAMPLE.COM", EmailConfirmed = true });
            _db.Users.Add(new AppUser { Id = 3, UserName = "unconfirmed", NormalizedUserName = "UNCONFIRMED", Email = "u@example.com", NormalizedEmail = "U@EXAMPLE.COM", EmailConfirmed = false });
            _db.SaveChanges();
            var handler = BuildHandler();

            var result = await handler.Handle(new GetUsersQuery(1, null), CancellationToken.None);

            var user = Assert.Single(result.Data!);
            Assert.Equal(2, user.Id);
        }

        [Fact]
        public async Task Handle_NoSearchTerm_ReturnsAllOtherConfirmedUsers()
        {
            _db.Users.Add(new AppUser { Id = 2, UserName = "alice", NormalizedUserName = "ALICE", Email = "alice@example.com", NormalizedEmail = "ALICE@EXAMPLE.COM", EmailConfirmed = true });
            _db.Users.Add(new AppUser { Id = 3, UserName = "bob", NormalizedUserName = "BOB", Email = "bob@example.com", NormalizedEmail = "BOB@EXAMPLE.COM", EmailConfirmed = true });
            _db.SaveChanges();
            var handler = BuildHandler();

            var result = await handler.Handle(new GetUsersQuery(1, null), CancellationToken.None);

            Assert.Equal(2, result.Data!.Count);
        }

        [Fact]
        public async Task Handle_WithSearchTermMatchingNormalizedUserNameOrEmail_ReturnsFilteredResults()
        {
            // Non-obvious production behavior: the handler filters NormalizedUserName/NormalizedEmail
            // against the raw SearchTerm value without upper-casing it - seed matching casing.
            _db.Users.Add(new AppUser { Id = 2, UserName = "alice99", NormalizedUserName = "ALICE99", Email = "alice@example.com", NormalizedEmail = "ALICE@EXAMPLE.COM", EmailConfirmed = true });
            _db.Users.Add(new AppUser { Id = 3, UserName = "bob", NormalizedUserName = "BOB", Email = "bob@example.com", NormalizedEmail = "BOB@EXAMPLE.COM", EmailConfirmed = true });
            _db.SaveChanges();
            var handler = BuildHandler();

            var result = await handler.Handle(new GetUsersQuery(1, "ALICE"), CancellationToken.None);

            var user = Assert.Single(result.Data!);
            Assert.Equal(2, user.Id);
        }

        [Fact]
        public async Task Handle_SearchTermNotMatchingAnyUser_ReturnsEmptyList()
        {
            _db.Users.Add(new AppUser { Id = 2, UserName = "alice", NormalizedUserName = "ALICE", Email = "alice@example.com", NormalizedEmail = "ALICE@EXAMPLE.COM", EmailConfirmed = true });
            _db.SaveChanges();
            var handler = BuildHandler();

            var result = await handler.Handle(new GetUsersQuery(1, "ZZZ"), CancellationToken.None);

            Assert.Empty(result.Data!);
        }
    }
}
