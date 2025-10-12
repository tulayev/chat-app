using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.CQRS.Messages.Queries;
using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.CQRS.Messages.Handlers
{
    public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, ApiResponse<IEnumerable<ChatMessageDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetChatMessagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<ChatMessageDto>>> Handle(GetChatMessagesQuery query, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetQueryable<Message>()
                .Include(x => x.Sender)
                .Where(x => x.ChatId == query.ChatId)
                .OrderBy(x => x.SentAt)
                .ToListAsync(cancellationToken);

            var mapped = _mapper.From(result).AdaptToType<IEnumerable<ChatMessageDto>>();

            return ApiResponse<IEnumerable<ChatMessageDto>>.Ok(mapped);
        }
    }
}
