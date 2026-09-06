using ChatApp.Application.Common.Interfaces.GoogleAuth;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.Login.Commands;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.CQRS.Login.Handlers
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, ApiResponse<string>>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<GoogleLoginCommandHandler> _logger;

        public GoogleLoginCommandHandler(IGoogleAuthService googleAuthService,
            IJwtTokenService jwtTokenService,
            UserManager<AppUser> userManager,
            ILogger<GoogleLoginCommandHandler> logger)
        {
            _googleAuthService = googleAuthService;
            _jwtTokenService = jwtTokenService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> Handle(GoogleLoginCommand command, CancellationToken cancellationToken)
        {
            var googleUser = await _googleAuthService.ValidateIdTokenAsync(command.GoogleLoginRequestDto.IdToken);

            var user = await _userManager.FindByEmailAsync(googleUser.Email);

            if (user is null)
            {
                user = new AppUser
                {
                    UserName = googleUser.UserName,
                    Email = googleUser.Email,
                    EmailConfirmed = googleUser.EmailVerified
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    var error = string.Join("; ", result.Errors.Select(e => e.Description));
                    return ApiResponse<string>.Fail($"Registration failed: {error}");
                }

                _logger.LogInformation("New user with username '{UserName}' has registered via Google!", user.UserName);
            }

            var token = _jwtTokenService.CreateToken(user);

            if (string.IsNullOrWhiteSpace(token))
            {
                return ApiResponse<string>.Fail("Something went wrong");
            }

            return ApiResponse<string>.Ok(token);
        }
    }
}
