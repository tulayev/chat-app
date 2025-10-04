using Core.CQRS.Message.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Extensions;

namespace ServerAPI.Controllers
{
    [Authorize]
    public class ChatController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("history/{withUserId}")]
        public async Task<IActionResult> GetHistory(int withUserId)
        {
            var currentUserId = User.GetUserId();
            var response = await _mediator.Send(new ChatHistoryQuery(currentUserId!, withUserId));

            return HandleResponse(response);
        }
    }
}
