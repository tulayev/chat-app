using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.CQRS.Messages.Commands;
using ChatApp.Application.Helpers;
using ChatApp.Application.Hubs;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.CQRS.Messages.Handlers
{
    public class MarkMessagesReadCommandHandler : IRequestHandler<MarkMessagesReadCommand, ApiResponse<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hubContext;

        public MarkMessagesReadCommandHandler(IUnitOfWork unitOfWork,
            IHubContext<ChatHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<Unit>> Handle(MarkMessagesReadCommand command, CancellationToken cancellationToken)
        {
            var unreadMessages = await _unitOfWork.GetQueryable<Message>()
                .Where(x => x.ChatId == command.ChatId && x.SenderId != command.ReaderId && x.ReadAt == null)
                .ToListAsync(cancellationToken);

            if (unreadMessages.Count == 0)
            {
                return ApiResponse<Unit>.Ok(Unit.Value);
            }

            var readAt = DateTime.UtcNow;

            foreach (var message in unreadMessages)
            {
                message.ReadAt = readAt;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _hubContext.Clients.Group($"chat-{command.ChatId}")
                .SendAsync("MessagesRead", new
                {
                    ChatId = command.ChatId,
                    ReadAt = readAt,
                    MessageIds = unreadMessages.Select(x => x.Id)
                }, cancellationToken);

            return ApiResponse<Unit>.Ok(Unit.Value);
        }
    }
}
