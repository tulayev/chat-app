using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.CQRS.Messages.Commands;
using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Models;
using MediatR;

namespace ChatApp.Application.CQRS.Messages.Handlers
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SendMessageDto> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                SenderId = command.SenderId,
                ReceiverId = command.ReceiverId,
                Content = command.Content,
            };

            await _unitOfWork.AddAsync(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SendMessageDto(message.Id, message.SenderId, message.ReceiverId, message.Content, message.SentAt);
        }
    }
}
