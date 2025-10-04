using Core.CQRS.Message.Commands;
using Core.Data.Repositories.Message;
using Core.Models.DTOs.Message;
using MediatR;

namespace Core.CQRS.Message.Handlers
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand, SendMessageDto>
    {
        private readonly IMessageRepository _messageRepository;

        public SendMessageHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<SendMessageDto> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var message = new Models.Message
            {
                SenderId = command.SenderId,
                ReceiverId = command.ReceiverId,
                Content = command.Content,
            };

            await _messageRepository.AddAsync(message);
            return new SendMessageDto(message.Id, message.SenderId, message.ReceiverId, message.Content, message.SentAt);
        }
    }
}
