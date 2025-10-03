using Core.CQRS.Login.Queries;
using Core.CQRS.Register.Commands;
using Core.Models.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<AuthResponse>> Register([FromForm] RegisterRequestDto request)
        {
            return Ok(await _mediator.Send(new RegisterUserCommand(request)));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequestDto request)
        {
            return Ok(await _mediator.Send(new LoginUserQuery(request)));
        }
    }
}
