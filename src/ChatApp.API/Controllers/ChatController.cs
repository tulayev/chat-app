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

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetChatMessages(int chatId)
        {
            var response = await _mediator.Send(new GetChatMessagesQuery(chatId));

            return HandleResponse(response);
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            command.SenderId = User.GetUserId();
            await _mediator.Send(command);

            return Ok();
        }
    }
}
