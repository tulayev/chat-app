using Core.CQRS.Message.Queries;
using Core.Data.Repositories.Message;
using MediatR;

namespace Core.CQRS.Message.Handlers
{
    public class ChatHistoryHandler : IRequestHandler<ChatHistoryQuery, IEnumerable<Models.Message>>
    {
        private readonly IMessageRepository _messageRepository;

        public ChatHistoryHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<IEnumerable<Models.Message>> Handle(ChatHistoryQuery query, CancellationToken cancellationToken)
        {
            return await _messageRepository.GetHistoryAsync(query.UserId, query.WithUserId);
        }
    }
}
