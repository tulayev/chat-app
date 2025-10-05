using Core.Data.Repositories.Message;
using Core.Data.Repositories.User;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Core.Data.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        public IUserRepository Users => _users ??= new UserRepository(_userManager); // Lazy initializing
        public IMessageRepository Messages => _messages ??= new MessageRepository(_db);

        private readonly ChatAppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private IUserRepository? _users;
        private IMessageRepository? _messages;

        public UnitOfWork(ChatAppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
