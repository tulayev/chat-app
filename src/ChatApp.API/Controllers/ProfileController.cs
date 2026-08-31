using ChatApp.Application.Common.Extensions;
using ChatApp.Application.CQRS.Profile.Commands;
using ChatApp.Application.DTOs.Profile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UpdateProfile([FromForm] UpdateProfileRequestDto request)
        {
            var userId = User.GetUserId();
            var response = await _mediator.Send(new UpdateProfileCommand(userId, request));

            return HandleResponse(response);
        }
    }
}
