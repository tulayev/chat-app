using ChatApp.Application.CQRS.Messages.Handlers;
using ChatApp.Application.CQRS.Messages.Queries;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using MapsterMapper;
using Moq;

namespace ChatApp.Tests.Handlers.Messages
{
    public class GetUserChatsQueryHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = TestDbContextFactory.Create();
        private readonly Mock<IMapper> _mapperMock = new();

        public GetUserChatsQueryHandlerTests()
        {
            _mapperMock.SetupGet(x => x.Config).Returns(MapsterTestConfig.Instance);
        }

        public void Dispose() => _db.Dispose();

        private GetUserChatsQueryHandler BuildHandler()
        {
            var uow = TestUnitOfWorkFactory.Create(_db);
            return new GetUserChatsQueryHandler(uow.Object, _mapperMock.Object);
        }

        private void SeedUser(int id) =>
            _db.Users.Add(new AppUser { Id = id, UserName = $"user{id}", Email = $"user{id}@example.com" });

        [Fact]
        public async Task Handle_NoChats_ReturnsEmptyList()
        {
            var handler = BuildHandler();

            var result = await handler.Handle(new GetUserChatsQuery(1), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Empty(result.Data!);
        }

        [Fact]
        public async Task Handle_MultipleChats_OrdersByLatestMessageDescending()
        {
            SeedUser(1);
            SeedUser(2);
            SeedUser(3);
            _db.Chats.Add(new Chat { Id = 1, User1Id = 1, User2Id = 2 });
            _db.Chats.Add(new Chat { Id = 2, User1Id = 1, User2Id = 3 });
            _db.SaveChanges();

            _db.Messages.Add(new Message { ChatId = 1, SenderId = 1, Content = "older", SentAt = DateTime.UtcNow.AddMinutes(-10) });
            _db.Messages.Add(new Message { ChatId = 2, SenderId = 1, Content = "newer", SentAt = DateTime.UtcNow });
            _db.SaveChanges();

            var handler = BuildHandler();

            var result = await handler.Handle(new GetUserChatsQuery(1), CancellationToken.None);

            var ordered = result.Data!.ToList();
            Assert.Equal(2, ordered.Count);
            Assert.Equal(2, ordered[0].ChatId);
            Assert.Equal(1, ordered[1].ChatId);
        }

        [Fact]
        public async Task Handle_ChatWithNoMessages_LastMessageIsNullAndLastMessageTimeIsDefaultDateTime()
        {
            SeedUser(1);
            SeedUser(2);
            _db.Chats.Add(new Chat { Id = 1, User1Id = 1, User2Id = 2 });
            _db.SaveChanges();

            var handler = BuildHandler();

            var result = await handler.Handle(new GetUserChatsQuery(1), CancellationToken.None);

            var chat = Assert.Single(result.Data!);
            Assert.Null(chat.LastMessage);
            Assert.Equal(default(DateTime), chat.LastMessageTime);
        }

        [Fact]
        public async Task Handle_CurrentUserIsUser1_ContactIsUser2()
        {
            SeedUser(1);
            SeedUser(2);
            _db.Chats.Add(new Chat { Id = 1, User1Id = 1, User2Id = 2 });
            _db.SaveChanges();

            var handler = BuildHandler();

            var result = await handler.Handle(new GetUserChatsQuery(1), CancellationToken.None);

            var chat = Assert.Single(result.Data!);
            Assert.Equal(2, chat.Contact.Id);
        }

        [Fact]
        public async Task Handle_CurrentUserIsUser2_ContactIsUser1()
        {
            SeedUser(1);
            SeedUser(2);
            _db.Chats.Add(new Chat { Id = 1, User1Id = 1, User2Id = 2 });
            _db.SaveChanges();

            var handler = BuildHandler();

            var result = await handler.Handle(new GetUserChatsQuery(2), CancellationToken.None);

            var chat = Assert.Single(result.Data!);
            Assert.Equal(1, chat.Contact.Id);
        }
    }
}
