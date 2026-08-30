using Microsoft.EntityFrameworkCore;

namespace ChatApp.Tests.TestHelpers
{
    public static class TestDbContextFactory
    {
        public static TestDbContext Create()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TestDbContext(options);
        }
    }
}
