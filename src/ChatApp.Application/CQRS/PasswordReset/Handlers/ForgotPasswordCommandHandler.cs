using ChatApp.Application.Common.Enums;
using ChatApp.Application.Common.Interfaces.Email;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.PasswordReset.Commands;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.CQRS.PasswordReset.Handlers
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<string>>
    {
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            IVerificationCodeService verificationCodeService,
            IEmailSenderService emailSenderService,
            UserManager<AppUser> userManager,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _verificationCodeService = verificationCodeService;
            _emailSenderService = emailSenderService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new Exception("User not found");

            var code = new Random().Next(100000, 999999).ToString();

            _logger.LogInformation($"PASSWORD RESET CODE: {code} sent to the {request.Email}");

            await _verificationCodeService.StoreCodeAsync(user.Email!, code, TimeSpan.FromMinutes(10), VerificationPurpose.PasswordReset);

            await _emailSenderService.SendAsync(user.Email!, "Password Reset", $"Your password reset code: {code}");

            return ApiResponse<string>.Ok("Password reset code sent");
        }
    }
}
