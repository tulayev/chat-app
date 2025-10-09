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
    public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, ApiResponse<IEnumerable<ChatHistoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetChatHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<ChatHistoryDto>>> Handle(GetChatHistoryQuery query, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetQueryable<Message>()
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Where(x =>
                    (x.SenderId == query.UserId && x.ReceiverId == query.WithUserId) ||
                    (x.SenderId == query.WithUserId && x.ReceiverId == query.UserId))
                .ToListAsync(cancellationToken);

            var mapped = _mapper.From(result).AdaptToType<IEnumerable<ChatHistoryDto>>();

            return ApiResponse<IEnumerable<ChatHistoryDto>>.Ok(mapped);
        }
    }
}
