using ChatApp.Application.Common.Extensions;
using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Client connected
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        // Client subscribed to a certain chat
        public async Task JoinChat(int chatId)
        {
            await EnsureParticipantAsync(chatId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }

        // Client leaves a chat
        public async Task LeaveChat(int chatId)
        {
            await EnsureParticipantAsync(chatId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }

        private async Task EnsureParticipantAsync(int chatId)
        {
            var userId = Context.User!.GetUserId();
            var isParticipant = await _unitOfWork.GetQueryable<Chat>()
                .AnyAsync(c => c.Id == chatId && (c.User1Id == userId || c.User2Id == userId));

            if (!isParticipant)
            {
                throw new HubException("You are not a participant of this chat.");
            }
        }
    }
}
