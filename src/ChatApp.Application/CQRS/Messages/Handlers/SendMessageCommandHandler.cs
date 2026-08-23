using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.CQRS.Messages.Commands;
using ChatApp.Application.DTOs.Message;
using ChatApp.Application.DTOs.User;
using ChatApp.Application.Helpers;
using ChatApp.Application.Hubs;
using ChatApp.Domain.Models;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.CQRS.Messages.Handlers
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ApiResponse<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IMapper _mapper;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork, IHubContext<ChatHub> hub, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _hub = hub;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Unit>> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                ChatId = command.ChatId,
                SenderId = command.SenderId,
                Content = command.Content!,
                SentAt = DateTime.UtcNow
            };

            await _unitOfWork.AddAsync(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var participants = await _unitOfWork.GetQueryable<Chat>()
                .Where(x => x.Id == command.ChatId)
                .Select(x => new
                {
                    Sender = x.User1Id == command.SenderId ? x.User1 : x.User2,
                    Receiver = x.User1Id == command.SenderId ? x.User2 : x.User1
                })
                .FirstOrDefaultAsync(cancellationToken);

            var senderDto = _mapper.Map<UserDto>(participants!.Sender);
            var receiverDto = _mapper.Map<UserDto>(participants.Receiver);
            var result = new MessageDto(message.Id, message.Content!, message.SentAt, senderDto, receiverDto);

            // Notify all in this chat
            await _hub.Clients.Group($"chat-{command.ChatId}").SendAsync("ReceiveMessage", result);

            return ApiResponse<Unit>.Ok(Unit.Value);
        }
    }
}
