using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.CQRS.Messages.Queries;
using ChatApp.Application.DTOs.Chat;
using ChatApp.Application.DTOs.User;
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
            var currentUserId = request.CurrentUserId;

            var chats = await _unitOfWork.GetQueryable<Chat>()
                .AsNoTracking()
                .Where(c => c.User1Id == currentUserId || c.User2Id == currentUserId)
                .OrderByDescending(c => c.Messages.OrderByDescending(x => x.SentAt).Select(x => x.SentAt).FirstOrDefault())
                .Select(x => new ChatDto
                (
                    x.Id,
                    new UserDto
                    (
                        currentUserId == x.User1Id ? x.User2Id : x.User1Id,
                        currentUserId == x.User1Id ? x.User2.UserName! : x.User1.UserName!,
                        currentUserId == x.User1Id ? x.User2.Email! : x.User1.Email!,
                        currentUserId == x.User1Id ? x.User2.AvatarUrl! : x.User1.AvatarUrl!
                    ),
                    x.Messages.OrderByDescending(x => x.SentAt).Select(x => x.Content).FirstOrDefault(),
                    x.Messages.OrderByDescending(x => x.SentAt).Select(x => x.SentAt).FirstOrDefault()
                ))
                .ToListAsync(cancellationToken);

            return ApiResponse<IEnumerable<ChatDto>>.Ok(chats);
        }
    }
}
