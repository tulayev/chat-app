using Core.CQRS.Message.Queries;
using Core.Data.Repositories.Message;
using Core.Helpers;
using Core.Models.DTOs.Message;
using MapsterMapper;
using MediatR;

namespace Core.CQRS.Message.Handlers
{
    public class ChatHistoryHandler : IRequestHandler<ChatHistoryQuery, ApiResponse<IEnumerable<MessageDto>>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public ChatHistoryHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<MessageDto>>> Handle(ChatHistoryQuery query, CancellationToken cancellationToken)
        {
            var result = await _messageRepository.GetHistoryAsync(query.UserId, query.WithUserId);
            var mapped = _mapper.From(result).AdaptToType<IEnumerable<MessageDto>>();

            return ApiResponse<IEnumerable<MessageDto>>.Ok(mapped);
        }
    }
}
