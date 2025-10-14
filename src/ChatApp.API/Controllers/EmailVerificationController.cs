using ChatApp.Application.CQRS.EmailVerification.Commands;
using ChatApp.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
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
            await _mediator.Send(request);
            return HandleResponse(ApiResponse<string>.Ok("Verification code sent"));
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyEmailCommand request)
        {
            await _mediator.Send(request);
            return HandleResponse(ApiResponse<string>.Ok("Email verified"));
        }
    }
}
