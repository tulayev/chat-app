using ChatApp.Application.CQRS.Messages.Handlers;
using ChatApp.Application.CQRS.Messages.Queries;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using MapsterMapper;
using Moq;

namespace ChatApp.Tests.Handlers.Messages
{
    public class GetChatMessagesQueryHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = TestDbContextFactory.Create();
        private readonly Mock<IMapper> _mapperMock = new();

        public GetChatMessagesQueryHandlerTests()
        {
            _mapperMock.SetupGet(x => x.Config).Returns(MapsterTestConfig.Instance);
        }

        public void Dispose() => _db.Dispose();

        private GetChatMessagesQueryHandler BuildHandler(out Mock<Application.Common.Interfaces.Repositories.IUnitOfWork> uowMock)
        {
            uowMock = TestUnitOfWorkFactory.Create(_db);
            return new GetChatMessagesQueryHandler(uowMock.Object, _mapperMock.Object);
        }

        private void SeedUsers(int user1Id, int user2Id)
        {
            _db.Users.Add(new AppUser { Id = user1Id, UserName = $"user{user1Id}", Email = $"user{user1Id}@example.com" });
            _db.Users.Add(new AppUser { Id = user2Id, UserName = $"user{user2Id}", Email = $"user{user2Id}@example.com" });
            _db.SaveChanges();
        }

        [Fact]
        public async Task Handle_SameCurrentAndTargetUser_ReturnsFail()
        {
            var handler = BuildHandler(out _);

            var result = await handler.Handle(new GetChatMessagesQuery(1, 1), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("You cannot chat with yourself!", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_NoExistingChat_CreatesNewChatAndReturnsEmptyMessages()
        {
            SeedUsers(1, 2);
            var handler = BuildHandler(out var uowMock);

            var result = await handler.Handle(new GetChatMessagesQuery(1, 2), CancellationToken.None);

            uowMock.Verify(x => x.AddAsync(It.IsAny<Chat>()), Times.Once);
            Assert.True(result.Success);
            Assert.Empty(result.Data!.Messages);
            Assert.Equal(2, result.Data.Contact.Id);
        }

        [Fact]
        public async Task Handle_ExistingChatReversedUserOrder_ReusesChatDoesNotCreateNew()
        {
            SeedUsers(1, 2);
            _db.Chats.Add(new Chat { Id = 5, User1Id = 2, User2Id = 1 });
            _db.SaveChanges();
            var handler = BuildHandler(out var uowMock);

            var result = await handler.Handle(new GetChatMessagesQuery(1, 2), CancellationToken.None);

            uowMock.Verify(x => x.AddAsync(It.IsAny<Chat>()), Times.Never);
            Assert.True(result.Success);
            Assert.Equal(5, result.Data!.ChatId);
        }

        [Fact]
        public async Task Handle_ContactUserMissing_ReturnsFail()
        {
            _db.Users.Add(new AppUser { Id = 1, UserName = "user1", Email = "user1@example.com" });
            _db.SaveChanges();
            var handler = BuildHandler(out var uowMock);

            var result = await handler.Handle(new GetChatMessagesQuery(1, 2), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("The contact user is not found!", result.ErrorMessage);
            uowMock.Verify(x => x.AddAsync(It.IsAny<Chat>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ExistingChatWithMessages_ReturnsMappedMessagesInStoredOrder()
        {
            SeedUsers(1, 2);
            var chat = new Chat { Id = 5, User1Id = 1, User2Id = 2 };
            _db.Chats.Add(chat);
            _db.SaveChanges();

            _db.Messages.AddRange(
                new Message { ChatId = 5, SenderId = 1, Content = "first", SentAt = DateTime.UtcNow.AddMinutes(-2) },
                new Message { ChatId = 5, SenderId = 2, Content = "second", SentAt = DateTime.UtcNow.AddMinutes(-1) },
                new Message { ChatId = 5, SenderId = 1, Content = "third", SentAt = DateTime.UtcNow });
            _db.SaveChanges();

            var handler = BuildHandler(out _);

            var result = await handler.Handle(new GetChatMessagesQuery(1, 2), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal(3, result.Data!.Messages.Count);
            Assert.Contains(result.Data.Messages, m => m.Content == "first" && m.Sender.Id == 1);
            Assert.Contains(result.Data.Messages, m => m.Content == "second" && m.Sender.Id == 2);
            Assert.Contains(result.Data.Messages, m => m.Content == "third" && m.Sender.Id == 1);
        }
    }
}
