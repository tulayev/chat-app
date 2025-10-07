using Core.CQRS.Messages.Queries;
using Core.Data.Repositories;
using Core.Helpers;
using Core.Models;
using Core.Models.DTOs.Message;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.CQRS.Messages.Handlers
{
    public class GetChatContactsQueryHandler : IRequestHandler<GetChatContactsQuery, ApiResponse<IEnumerable<ChatContactDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChatContactsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<ChatContactDto>>> Handle(GetChatContactsQuery request, CancellationToken cancellationToken)
        {
            var userId = request.CurrentUserId;

            var contacts = await _unitOfWork.GetQueryable<Message>()
                .Where(m => m.ReceiverId == userId || m.SenderId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMessageDate = g.Max(m => m.SentAt),
                    LastMessage = g.OrderByDescending(m => m.SentAt).Select(m => m.Content).FirstOrDefault()
                })
                .OrderByDescending(x => x.LastMessageDate)
                .Join(_unitOfWork.GetQueryable<AppUser>(),
                      msg => msg.UserId,
                      u => u.Id,
                      (msg, u) => new
                      {
                          user = u,
                          msg.LastMessage,
                          msg.LastMessageDate
                      })
                .Select(x => new ChatContactDto
                {
                    Id = x.user.Id,
                    Username = x.user.UserName,
                    LastMessage = x.LastMessage,
                    LastMessageDate = x.LastMessageDate
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<IEnumerable<ChatContactDto>>.Ok(contacts);
        }
    }
}
