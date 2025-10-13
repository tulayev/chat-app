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
                SentAt = DateTime.Now
            };

            await _unitOfWork.AddAsync(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var user = await _unitOfWork.GetQueryable<AppUser>().FirstOrDefaultAsync(x => x.Id == message.SenderId);
            var userDto = _mapper.Map<UserDto>(user!);
            var result = new ChatMessageDto(message.Id, message.ChatId, userDto, message.Content!, message.SentAt);

            // Notify all in this chat
            await _hub.Clients.Group($"chat-{command.ChatId}").SendAsync("ReceiveMessage", result);

            return ApiResponse<Unit>.Ok(Unit.Value);
        }
    }
}
