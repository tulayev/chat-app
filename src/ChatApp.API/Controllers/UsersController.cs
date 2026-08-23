using ChatApp.API.Extensions;
using ChatApp.Application.CQRS.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUsers([FromQuery] string? search)
        {
            var response = await _mediator.Send(new GetUsersQuery(User.GetUserId(), search?.ToLower()));
            return HandleResponse(response);
        }
    }
}
