using ChatApp.Application.Common.Enums;

namespace ChatApp.Application.Common.Interfaces.Security
{
    public interface IVerificationCodeService
    {
        Task StoreCodeAsync(string email, string code, TimeSpan lifetime, VerificationPurpose purpose = VerificationPurpose.EmailVerification);
        Task<string?> GetCodeAsync(string email, VerificationPurpose purpose = VerificationPurpose.EmailVerification);
        Task DeleteCodeAsync(string email, VerificationPurpose purpose = VerificationPurpose.EmailVerification);
    }
}
