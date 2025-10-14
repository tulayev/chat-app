namespace ChatApp.Application.Common.Interfaces.Security
{
    public interface IVerificationCodeService
    {
        Task StoreCodeAsync(string email, string code, TimeSpan lifetime);
        Task<string?> GetCodeAsync(string email);
        Task DeleteCodeAsync(string email);
    }
}
