using Core.CQRS.Messages.Commands;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using ServerAPI.Extensions;

namespace ServerAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendPrivateMessage(int toUserId, string message)
        {
            var senderId = Context.User!.GetUserId();

            var result = await _mediator.Send(new SendMessageCommand(senderId!, toUserId, message));

            await Clients.User(toUserId.ToString()).SendAsync("ReceiveMessage", result.SenderId, result.Content, result.SentAt);
            await Clients.User(senderId.ToString()).SendAsync("ReceiveMessage", result.SenderId, result.Content, result.SentAt);
        }
    }
}
