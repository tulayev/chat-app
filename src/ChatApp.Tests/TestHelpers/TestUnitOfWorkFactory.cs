using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Domain.Models;
using Moq;

namespace ChatApp.Tests.TestHelpers
{
    public static class TestUnitOfWorkFactory
    {
        public static Mock<IUnitOfWork> Create(TestDbContext db)
        {
            var mock = new Mock<IUnitOfWork>();

            mock.Setup(u => u.GetQueryable<AppUser>()).Returns(() => db.Users);
            mock.Setup(u => u.GetQueryable<Chat>()).Returns(() => db.Chats);
            mock.Setup(u => u.GetQueryable<Message>()).Returns(() => db.Messages);

            mock.Setup(u => u.AddAsync(It.IsAny<AppUser>())).Returns<AppUser>(e => { db.Users.Add(e); return Task.CompletedTask; });
            mock.Setup(u => u.AddAsync(It.IsAny<Chat>())).Returns<Chat>(e => { db.Chats.Add(e); return Task.CompletedTask; });
            mock.Setup(u => u.AddAsync(It.IsAny<Message>())).Returns<Message>(e => { db.Messages.Add(e); return Task.CompletedTask; });

            mock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(ct => db.SaveChangesAsync(ct));

            return mock;
        }
    }
}
