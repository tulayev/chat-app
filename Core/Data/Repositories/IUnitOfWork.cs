using Core.Data.Repositories.Message;
using Core.Data.Repositories.User;

namespace Core.Data.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IMessageRepository Messages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
