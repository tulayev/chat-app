using Core.Models;
using Core.Models.DTOs.Message;
using Mapster;

namespace Core.Mappings
{
    public class MessageMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Message, MessageDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.SentTime, src => src.SentAt.DateTime)
                .Map(dest => dest.From, src => src.Sender)
                .Map(dest => dest.To, src => src.Receiver);
        }
    }
}
