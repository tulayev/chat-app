namespace ChatApp.Application.Common.Interfaces.GoogleAuth
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken);
    }

    public record GoogleUserInfo(string Email, string UserName, bool EmailVerified);
}
