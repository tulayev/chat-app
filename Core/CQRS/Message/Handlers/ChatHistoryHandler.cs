using Core.CQRS.Message.Queries;
using Core.Data.Repositories.Message;
using Core.Helpers;
using MediatR;

namespace Core.CQRS.Message.Handlers
{
    public class ChatHistoryHandler : IRequestHandler<ChatHistoryQuery, ApiResponse<IEnumerable<Models.Message>>>
    {
        private readonly IMessageRepository _messageRepository;

        public ChatHistoryHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<ApiResponse<IEnumerable<Models.Message>>> Handle(ChatHistoryQuery query, CancellationToken cancellationToken)
        {
            var result = await _messageRepository.GetHistoryAsync(query.UserId, query.WithUserId);
            return ApiResponse<IEnumerable<Models.Message>>.Ok(result);
        }
    }
}
