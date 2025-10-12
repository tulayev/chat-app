using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.CQRS.Messages.Commands;
using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Hubs;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Application.CQRS.Messages.Handlers
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hub;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork, IHubContext<ChatHub> hub)
        {
            _unitOfWork = unitOfWork;
            _hub = hub;
        }

        public async Task<SendMessageDto> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                SenderId = command.SenderId,
                Content = command.Content!,
                SentAt = DateTime.Now
            };

            await _unitOfWork.AddAsync(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify all in this chat
            await _hub.Clients.Group($"chat-{command.ChatId}").SendAsync("ReceiveMessage", new
            {
                message.ChatId,
                message.SenderId,
                message.Content,
                message.SentAt
            });

            return new SendMessageDto(message.Id, message.SenderId, message.Content!, message.SentAt);
        }
    }
}
