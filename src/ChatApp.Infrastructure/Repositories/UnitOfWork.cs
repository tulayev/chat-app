using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ChatAppDbContext _db;

        public UnitOfWork(ChatAppDbContext db)
        {
            _db = db;
        }

        public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            return _db.Set<TEntity>();
        }

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await _db.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            await _db.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Set<TEntity>().Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Set<TEntity>().Remove(entity);
        }

        public void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _db.Set<TEntity>().RemoveRange(entities);
        }

        public void DeleteRange<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            _db.Set<TEntity>().RemoveRange(_db.Set<TEntity>().Where(predicate));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
