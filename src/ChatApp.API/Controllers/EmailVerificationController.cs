using ChatApp.API.Extensions;
using ChatApp.Application.CQRS.EmailVerification.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ChatApp.API.Controllers
{
    [EnableRateLimiting(AppServicesExtensions.VerificationRateLimiterPolicy)]
    public class EmailVerificationController : BaseApiController
    {
        private readonly IMediator _mediator;

        public EmailVerificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendVerificationCodeCommand request)
        {
            var response = await _mediator.Send(request);
            return HandleResponse(response);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyEmailCommand request)
        {
            var response = await _mediator.Send(request);
            return HandleResponse(response);
        }
    }
}
