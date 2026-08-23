using ChatApp.API.Extensions;
using ChatApp.Application.CQRS.Messages.Commands;
using ChatApp.Application.CQRS.Messages.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Authorize]
    public class ChatController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("userChats")]
        public async Task<IActionResult> GetUserChats()
        {
            var response = await _mediator.Send(new GetUserChatsQuery(User.GetUserId()));
            return HandleResponse(response);
        }

        [HttpGet("messages/with/{userId}")]
        public async Task<IActionResult> GetChatMessages(int userId)
        {
            var response = await _mediator.Send(new GetChatMessagesQuery(User.GetUserId(), userId));
            return HandleResponse(response);
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            command.SenderId = User.GetUserId();
            var response = await _mediator.Send(command);

            return HandleResponse(response);
        }
    }
}
