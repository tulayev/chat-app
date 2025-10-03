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
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Models.Message>> GetHistoryAsync(int userId, int withUserId)
        {
            return await _db.Messages
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == withUserId) ||
                    (m.SenderId == withUserId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
