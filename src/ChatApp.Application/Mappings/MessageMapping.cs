using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Models;
using Mapster;

namespace ChatApp.Application.Mappings
{
    public class MessageMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Message, ChatMessageDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.ChatId, src => src.ChatId)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.SentAt, src => src.SentAt)
                .Map(dest => dest.Sender, src => src.Sender);
        }
    }
}
