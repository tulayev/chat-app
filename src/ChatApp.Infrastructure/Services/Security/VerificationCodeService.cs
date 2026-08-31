using ChatApp.Application.Common.Enums;
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

        public async Task StoreCodeAsync(string email, string code, TimeSpan lifetime, VerificationPurpose purpose = VerificationPurpose.EmailVerification)
        {
            await _db.StringSetAsync(NormalizeKey(email, purpose), code, lifetime);
        }

        public async Task<string?> GetCodeAsync(string email, VerificationPurpose purpose = VerificationPurpose.EmailVerification)
        {
            var value = await _db.StringGetAsync(NormalizeKey(email, purpose));
            return value.HasValue ? value.ToString() : null;
        }

        public async Task DeleteCodeAsync(string email, VerificationPurpose purpose = VerificationPurpose.EmailVerification)
        {
            await _db.KeyDeleteAsync(NormalizeKey(email, purpose));
        }

        private static string NormalizeKey(string email, VerificationPurpose purpose)
        {
            return $"{KeyPrefix(purpose)}:{email.Trim().ToLowerInvariant()}";
        }

        private static string KeyPrefix(VerificationPurpose purpose) => purpose switch
        {
            VerificationPurpose.EmailVerification => "verify",
            VerificationPurpose.PasswordReset => "reset",
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), purpose, null)
        };
    }
}
