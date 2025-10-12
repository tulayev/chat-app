using ChatApp.Application.DTOs.Chat;
using ChatApp.Domain.Models;
using Mapster;

namespace ChatApp.Application.Mappings
{
    public class ChatMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<(Chat chat, int currentUserId), ChatDto>()
                .Map(dest => dest.ChatId, src => src.chat.Id)
                .Map(dest => dest.Contact, src => src.currentUserId == src.chat.User1Id ? src.chat.User2 : src.chat.User1)
                .Map(dest => dest.LastMessage, src =>
                    src.chat.Messages.OrderByDescending(m => m.SentAt)
                        .Select(m => m.Content)
                        .FirstOrDefault())
                .Map(dest => dest.LastMessageTime, src =>
                    src.chat.Messages.OrderByDescending(m => m.SentAt)
                        .Select(m => m.SentAt)
                        .FirstOrDefault());
        }
    }
}
