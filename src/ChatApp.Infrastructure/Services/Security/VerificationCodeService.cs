using ChatApp.Application.Common.Interfaces.Security;
using StackExchange.Redis;

namespace ChatApp.Infrastructure.Services.Security
{
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly IDatabase _db;

        public VerificationCodeService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task StoreCodeAsync(string email, string code, TimeSpan lifetime)
        {
            await _db.StringSetAsync(NormalizeKey(email), code, lifetime);
        }

        public async Task<string?> GetCodeAsync(string email)
        {
            var value = await _db.StringGetAsync(NormalizeKey(email));
            return value.HasValue ? value.ToString() : null;
        }

        public async Task DeleteCodeAsync(string email)
        {
            await _db.KeyDeleteAsync(NormalizeKey(email));
        }

        private static string NormalizeKey(string email)
        {
            return $"verify:{email.Trim().ToLowerInvariant()}";
        }
    }
}
