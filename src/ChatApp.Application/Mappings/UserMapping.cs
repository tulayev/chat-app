using ChatApp.Application.DTOs.User;
using ChatApp.Domain.Models;
using Mapster;

namespace ChatApp.Application.Mappings
{
    public class UserMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AppUser, UserDto>()
                .Map(dest => dest.Username, src => src.UserName);
        }
    }
}
