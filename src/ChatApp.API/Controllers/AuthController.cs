using ChatApp.Application.Common.Extensions;
using ChatApp.Application.CQRS.Login.Queries;
using ChatApp.Application.CQRS.Register.Commands;
using ChatApp.Application.CQRS.Users.Queries;
using ChatApp.Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> Register([FromForm] RegisterRequestDto request)
        {
            var response = await _mediator.Send(new RegisterUserCommand(request));
            return HandleResponse(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequestDto request)
        {
            var response = await _mediator.Send(new LoginUserQuery(request));
            return HandleResponse(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult> GetMe()
        {
            var userId = User.GetUserId();
            var response = await _mediator.Send(new GetAuthenticatedUserQuery(userId));

            return HandleResponse(response);
        }
    }
}
