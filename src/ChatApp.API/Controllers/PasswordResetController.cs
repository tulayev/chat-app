using ChatApp.API.Extensions;
using ChatApp.Application.CQRS.PasswordReset.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ChatApp.API.Controllers
{
    [EnableRateLimiting(AppServicesExtensions.VerificationRateLimiterPolicy)]
    public class PasswordResetController : BaseApiController
    {
        private readonly IMediator _mediator;

        public PasswordResetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> Forgot([FromBody] ForgotPasswordCommand request)
        {
            var response = await _mediator.Send(request);
            return HandleResponse(response);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordCommand request)
        {
            var response = await _mediator.Send(request);
            return HandleResponse(response);
        }
    }
}
