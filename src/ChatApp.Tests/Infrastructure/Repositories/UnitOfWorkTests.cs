using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Tests.Infrastructure.Repositories
{
    // Exercises AppUser/Message only. Chat is excluded: ChatConfiguration's Postgres-only
    // HasComputedColumnSql("LEAST"/"GREATEST") shadow columns silently stay at their CLR default
    // under the InMemory provider, causing spurious unique-index violations on a second row that
    // have nothing to do with UnitOfWork's own logic.
    public class UnitOfWorkTests : IDisposable
    {
        private readonly string _dbName = Guid.NewGuid().ToString();
        private readonly ChatAppDbContext _db;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            _db = CreateContext();
            _unitOfWork = new UnitOfWork(_db);
        }

        private ChatAppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ChatAppDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;
            return new ChatAppDbContext(options);
        }

        public void Dispose() => _db.Dispose();

        private static AppUser BuildUser(int id) => new()
        {
            Id = id,
            UserName = $"user{id}",
            NormalizedUserName = $"USER{id}",
            Email = $"user{id}@example.com",
            NormalizedEmail = $"USER{id}@EXAMPLE.COM"
        };

        [Fact]
        public async Task GetQueryable_ReturnsAllPersistedEntitiesOfType()
        {
            _db.Users.Add(BuildUser(1));
            _db.Users.Add(BuildUser(2));
            await _db.SaveChangesAsync();

            var result = _unitOfWork.GetQueryable<AppUser>().ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddAsync_ThenSaveChangesAsync_PersistsEntity()
        {
            await _unitOfWork.AddAsync(BuildUser(1));
            await _unitOfWork.SaveChangesAsync();

            using var verifyDb = CreateContext();
            Assert.Single(verifyDb.Users);
        }

        [Fact]
        public async Task AddRangeAsync_ThenSaveChangesAsync_PersistsAllEntities()
        {
            await _unitOfWork.AddRangeAsync(new[] { BuildUser(1), BuildUser(2) });
            await _unitOfWork.SaveChangesAsync();

            using var verifyDb = CreateContext();
            Assert.Equal(2, verifyDb.Users.Count());
        }

        [Fact]
        public async Task Update_ThenSaveChangesAsync_PersistsModifiedEntity()
        {
            _db.Users.Add(BuildUser(1));
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            using var loadContext = CreateContext();
            var detached = loadContext.Users.AsNoTracking().Single(x => x.Id == 1);
            detached.UserName = "renamed";

            _unitOfWork.Update(detached);
            await _unitOfWork.SaveChangesAsync();

            using var verifyDb = CreateContext();
            var reloaded = verifyDb.Users.Single(x => x.Id == 1);
            Assert.Equal("renamed", reloaded.UserName);
        }

        [Fact]
        public async Task Delete_ThenSaveChangesAsync_RemovesEntity()
        {
            var user = BuildUser(1);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _unitOfWork.Delete(user);
            await _unitOfWork.SaveChangesAsync();

            using var verifyDb = CreateContext();
            Assert.Empty(verifyDb.Users);
        }

        [Fact]
        public async Task DeleteRange_IEnumerableOverload_ThenSaveChangesAsync_RemovesAllGivenEntities()
        {
            var user1 = BuildUser(1);
            var user2 = BuildUser(2);
            _db.Users.AddRange(user1, user2);
            await _db.SaveChangesAsync();

            _unitOfWork.DeleteRange(new[] { user1, user2 });
            await _unitOfWork.SaveChangesAsync();

            using var verifyDb = CreateContext();
            Assert.Empty(verifyDb.Users);
        }

        [Fact]
        public async Task DeleteRange_PredicateOverload_ThenSaveChangesAsync_RemovesMatchingEntities()
        {
            var confirmed = BuildUser(1);
            confirmed.EmailConfirmed = true;
            var unconfirmed = BuildUser(2);
            unconfirmed.EmailConfirmed = false;
            _db.Users.AddRange(confirmed, unconfirmed);
            await _db.SaveChangesAsync();

            _unitOfWork.DeleteRange<AppUser>(x => x.EmailConfirmed);
            await _unitOfWork.SaveChangesAsync();

            using var verifyDb = CreateContext();
            var remaining = Assert.Single(verifyDb.Users);
            Assert.Equal(2, remaining.Id);
        }

        [Fact]
        public async Task SaveChangesAsync_ReturnsCountOfAffectedRows()
        {
            await _unitOfWork.AddAsync(BuildUser(1));
            await _unitOfWork.AddAsync(BuildUser(2));

            var count = await _unitOfWork.SaveChangesAsync();

            Assert.Equal(2, count);
        }
    }
}
