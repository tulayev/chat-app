using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Application.Hubs
{
    public class ChatHub : Hub
    {
        // Client connected
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        // Client subscribed to a certain chat
        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }

        // Client leaves a chat
        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }
    }
}
