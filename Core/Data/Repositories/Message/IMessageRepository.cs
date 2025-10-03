namespace Core.Data.Repositories.Message
{
    public interface IMessageRepository
    {
        Task AddAsync(Models.Message message);
        Task<IEnumerable<Models.Message>> GetHistoryAsync(int userId, int withUserId);
    }
}
