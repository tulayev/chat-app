using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.EmailVerification.Commands;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.CQRS.EmailVerification.Handlers
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
    {
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly UserManager<AppUser> _userManager;

        public VerifyEmailCommandHandler(IVerificationCodeService verificationCodeService, UserManager<AppUser> userManager)
        {
            _verificationCodeService = verificationCodeService;
            _userManager = userManager;
        }

        public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var storedCode = await _verificationCodeService.GetCodeAsync(request.Email) 
                ?? throw new InvalidOperationException("Code expired or not found");
            
            if (storedCode != request.Code)
            {
                throw new Exception("Invalid verification code");
            }

            var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exception("User not found");

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            await _verificationCodeService.DeleteCodeAsync(request.Email);
        }
    }
}
