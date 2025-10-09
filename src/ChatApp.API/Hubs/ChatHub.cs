using ChatApp.API.Extensions;
using ChatApp.Application.CQRS.Messages.Commands;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendPrivateMessage(int receiverId, string content)
        {
            var senderId = Context.User!.GetUserId();

            var result = await _mediator.Send(new SendMessageCommand(senderId!, receiverId, content));

            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", result.SenderId, result.Content, result.SentAt);
            await Clients.User(senderId.ToString()).SendAsync("ReceiveMessage", result.SenderId, result.Content, result.SentAt);
        }
    }
}
