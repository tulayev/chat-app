using Core.CQRS.Messages.Queries;
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

        [HttpGet("contacts")]
        public async Task<IActionResult> GetChatContacts()
        {
            var currentUserId = User.GetUserId();
            var response = await _mediator.Send(new GetChatContactsQuery(currentUserId));

            return HandleResponse(response);
        }

        [HttpGet("history/{withUserId}")]
        public async Task<IActionResult> GetChatHistory(int withUserId)
        {
            var currentUserId = User.GetUserId();
            var response = await _mediator.Send(new GetChatHistoryQuery(currentUserId!, withUserId));

            return HandleResponse(response);
        }
    }
}
