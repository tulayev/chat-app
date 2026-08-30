using ChatApp.Application.CQRS.Messages.Commands;
using ChatApp.Application.CQRS.Messages.Handlers;
using ChatApp.Application.Hubs;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace ChatApp.Tests.Handlers.Messages
{
    public class SendMessageCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = TestDbContextFactory.Create();
        private readonly Mock<IHubContext<ChatHub>> _hubMock = new();
        private readonly Mock<IHubClients> _clientsMock = new();
        private readonly Mock<IClientProxy> _clientProxyMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        public SendMessageCommandHandlerTests()
        {
            _hubMock.Setup(x => x.Clients).Returns(_clientsMock.Object);
            _clientsMock.Setup(x => x.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _mapperMock.SetupGet(x => x.Config).Returns(MapsterTestConfig.Instance);
        }

        public void Dispose() => _db.Dispose();

        private SendMessageCommandHandler BuildHandler()
        {
            var uow = TestUnitOfWorkFactory.Create(_db);
            return new SendMessageCommandHandler(uow.Object, _hubMock.Object, _mapperMock.Object);
        }

        private void SeedChat(int chatId, int user1Id, int user2Id)
        {
            _db.Users.Add(new AppUser { Id = user1Id, UserName = $"user{user1Id}", Email = $"user{user1Id}@example.com" });
            _db.Users.Add(new AppUser { Id = user2Id, UserName = $"user{user2Id}", Email = $"user{user2Id}@example.com" });
            _db.Chats.Add(new Chat { Id = chatId, User1Id = user1Id, User2Id = user2Id });
            _db.SaveChanges();
        }

        [Fact]
        public async Task Handle_SenderIdDoesNotMatchUser1Id_FallsBackToUser2AsBroadcastSender()
        {
            // Documents actual (non-obvious) production behavior: the sender-resolution ternary
            // `x.User1Id == command.SenderId ? x.User1 : x.User2` has no real "else" branch validating
            // participancy - any SenderId other than User1Id resolves to User2, even if SenderId matches
            // neither participant. This does NOT produce a Fail result.
            SeedChat(chatId: 1, user1Id: 10, user2Id: 20);
            var handler = BuildHandler();
            var command = new SendMessageCommand { ChatId = 1, SenderId = 999, Content = "hi" };
            object[]? broadcastArgs = null;
            _clientProxyMock.Setup(x => x.SendCoreAsync("ReceiveMessage", It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Callback<string, object[], CancellationToken>((_, args, _) => broadcastArgs = args)
                .Returns(Task.CompletedTask);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            Assert.NotNull(broadcastArgs);
            var messageDto = Assert.IsType<ChatApp.Application.DTOs.Message.MessageDto>(broadcastArgs![0]);
            Assert.Equal(20, messageDto.Sender.Id);
        }

        [Fact]
        public async Task Handle_ChatIdDoesNotExist_ReturnsFail()
        {
            var handler = BuildHandler();
            var command = new SendMessageCommand { ChatId = 999, SenderId = 10, Content = "hi" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("User does not exist or not authenticated!", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ValidMessage_PersistsMessageViaUnitOfWork()
        {
            SeedChat(chatId: 1, user1Id: 10, user2Id: 20);
            var handler = BuildHandler();
            var command = new SendMessageCommand { ChatId = 1, SenderId = 10, Content = "hello" };

            await handler.Handle(command, CancellationToken.None);

            var persisted = Assert.Single(_db.Messages.ToList());
            Assert.Equal(1, persisted.ChatId);
            Assert.Equal(10, persisted.SenderId);
            Assert.Equal("hello", persisted.Content);
        }

        [Fact]
        public async Task Handle_ValidMessage_SetsSentAtToUtcNowWithinTolerance()
        {
            SeedChat(chatId: 1, user1Id: 10, user2Id: 20);
            var handler = BuildHandler();
            var command = new SendMessageCommand { ChatId = 1, SenderId = 10, Content = "hello" };

            var before = DateTime.UtcNow;
            await handler.Handle(command, CancellationToken.None);

            var persisted = Assert.Single(_db.Messages.ToList());
            Assert.True(Math.Abs((DateTime.UtcNow - persisted.SentAt).TotalSeconds) < 5);
            Assert.True(persisted.SentAt >= before.AddSeconds(-1));
        }

        [Fact]
        public async Task Handle_ValidMessage_ReturnsOkUnit()
        {
            SeedChat(chatId: 1, user1Id: 10, user2Id: 20);
            var handler = BuildHandler();
            var command = new SendMessageCommand { ChatId = 1, SenderId = 10, Content = "hello" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Handle_ValidMessage_BroadcastsToCorrectChatGroup()
        {
            SeedChat(chatId: 1, user1Id: 10, user2Id: 20);
            var handler = BuildHandler();
            var command = new SendMessageCommand { ChatId = 1, SenderId = 10, Content = "hello" };

            await handler.Handle(command, CancellationToken.None);

            _clientsMock.Verify(x => x.Group("chat-1"), Times.Once);
            _clientProxyMock.Verify(x => x.SendCoreAsync("ReceiveMessage", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
