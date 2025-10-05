using Core.CQRS.Message.Commands;
using Core.Data.Repositories;
using Core.Models.DTOs.Message;
using MediatR;

namespace Core.CQRS.Message.Handlers
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand, SendMessageDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendMessageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SendMessageDto> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var message = new Models.Message
            {
                SenderId = command.SenderId,
                ReceiverId = command.ReceiverId,
                Content = command.Content,
            };

            await _unitOfWork.Messages.AddAsync(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SendMessageDto(message.Id, message.SenderId, message.ReceiverId, message.Content, message.SentAt);
        }
    }
}
