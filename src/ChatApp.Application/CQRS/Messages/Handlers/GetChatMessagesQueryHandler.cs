using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.CQRS.Messages.Queries;
using ChatApp.Application.DTOs.Chat;
using ChatApp.Application.DTOs.Message;
using ChatApp.Application.DTOs.User;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.CQRS.Messages.Handlers
{
    public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, ApiResponse<ChatMessagesDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetChatMessagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ChatMessagesDto>> Handle(GetChatMessagesQuery query, CancellationToken cancellationToken)
        {
            var currentUserId = query.CurrentUserId;
            var receiverUserId = query.UserId;

            if (currentUserId == receiverUserId)
            {
                return ApiResponse<ChatMessagesDto>.Fail("You cannot chat with yourself!");
            }

            var chat = await _unitOfWork.GetQueryable<Chat>().FirstOrDefaultAsync(x =>
                (x.User1Id == currentUserId && x.User2Id == receiverUserId)
                || (x.User1Id == receiverUserId && x.User2Id == currentUserId), cancellationToken);

            if (chat is null)
            {
                chat = new Chat
                {
                    User1Id = currentUserId,
                    User2Id = receiverUserId,
                };

                await _unitOfWork.AddAsync(chat);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var contact = await _unitOfWork.GetQueryable<AppUser>()
                .FirstAsync(x => x.Id == receiverUserId, cancellationToken);
            var contactDto = _mapper.Map<UserDto>(contact);

            var messages = await _unitOfWork.GetQueryable<Chat>()
                .Where(x => x.Id == chat.Id)
                .SelectMany(x => x.Messages)
                .ProjectToType<MessageDto>(_mapper.Config)
                .ToListAsync(cancellationToken);

            return ApiResponse<ChatMessagesDto>.Ok(new ChatMessagesDto(chat.Id, contactDto, messages));
        }
    }
}
