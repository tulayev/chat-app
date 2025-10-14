using ChatApp.Application.Common.Interfaces.Email;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.EmailVerification.Commands;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.CQRS.EmailVerification.Handlers
{
    public class SendVerificationCodeCommandHandler : IRequestHandler<SendVerificationCodeCommand>
    {
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IEmailSenderService _emailSenderService; 
        private readonly UserManager<AppUser> _userManager;

        public SendVerificationCodeCommandHandler(
            IVerificationCodeService verificationCodeService,
            IEmailSenderService emailSenderService,
            UserManager<AppUser> userManager)
        {
            _verificationCodeService = verificationCodeService;
            _emailSenderService = emailSenderService;
            _userManager = userManager;
        }

        public async Task Handle(SendVerificationCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email) 
                ?? throw new Exception("User not found");
            
            var code = new Random().Next(100000, 999999).ToString();

            await _verificationCodeService.StoreCodeAsync(user.Email!, code, TimeSpan.FromMinutes(10));

            await _emailSenderService.SendAsync(user.Email!, "Email Verification", $"Your verification code: {code}");
        }
    }
}
