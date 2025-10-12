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

        // Sending message (called from frontend)
        public async Task SendMessage(int chatId, int senderId, string content)
        {
            await Clients.Group($"chat-{chatId}").SendAsync("ReceiveMessage", new
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                SentAt = DateTime.Now
            });
        }
    }
}
