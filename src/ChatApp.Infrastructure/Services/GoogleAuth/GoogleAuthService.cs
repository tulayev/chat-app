using ChatApp.Application.Common.Interfaces.GoogleAuth;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Infrastructure.Services.GoogleAuth
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly string _clientId;

        public GoogleAuthService(IConfiguration configuration)
        {
            _clientId = configuration["Google:ClientId"]
                ?? throw new InvalidOperationException("Google's client id is not configured.");
        }

        public async Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            });

            return new GoogleUserInfo(payload.Email, payload.GivenName, payload.EmailVerified);
        }
    }
}
