using ChatApp.Application.Common.Enums;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.PasswordReset.Commands;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.CQRS.PasswordReset.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<string>>
    {
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly UserManager<AppUser> _userManager;

        public ResetPasswordCommandHandler(IVerificationCodeService verificationCodeService, UserManager<AppUser> userManager)
        {
            _verificationCodeService = verificationCodeService;
            _userManager = userManager;
        }

        public async Task<ApiResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var storedCode = await _verificationCodeService.GetCodeAsync(request.Email, VerificationPurpose.PasswordReset)
                ?? throw new InvalidOperationException("Code expired or not found");

            if (storedCode != request.Code)
            {
                throw new Exception("Invalid verification code");
            }

            var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exception("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
            {
                var error = string.Join("; ", result.Errors.Select(e => e.Description));
                return ApiResponse<string>.Fail($"Password reset failed: {error}");
            }

            await _verificationCodeService.DeleteCodeAsync(request.Email, VerificationPurpose.PasswordReset);

            return ApiResponse<string>.Ok("Password reset successful");
        }
    }
}
