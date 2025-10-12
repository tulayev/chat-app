using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.CQRS.Messages.Queries;
using ChatApp.Application.DTOs.Chat;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.CQRS.Messages.Handlers
{
    public class GetUserChatsQueryHandler : IRequestHandler<GetUserChatsQuery, ApiResponse<IEnumerable<ChatDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserChatsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<ChatDto>>> Handle(GetUserChatsQuery request, CancellationToken cancellationToken)
        {
            var userId = request.CurrentUserId;

            var chats = await _unitOfWork.GetQueryable<Chat>()
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync(cancellationToken);

            var result = chats
                .Select(chat => _mapper.Map<(Chat chat, int currentUserId), ChatDto>((chat, userId)))
                .OrderByDescending(x => x.LastMessageTime)
                .ToList();

            return ApiResponse<IEnumerable<ChatDto>>.Ok(result);
        }
    }
}
