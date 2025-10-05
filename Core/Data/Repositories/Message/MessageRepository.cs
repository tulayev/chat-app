using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.Repositories.Message
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatAppDbContext _db;

        public MessageRepository(ChatAppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.Message message)
        {
            await _db.Messages.AddAsync(message);
        }

        public async Task<IEnumerable<Models.Message>> GetHistoryAsync(int userId, int withUserId)
        {
            return await _db.Messages
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Where(MessageExtensions.BetweenUsersPredicate(userId, withUserId))
                .ToListAsync();
        }
    }
}
