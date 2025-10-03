using Core.Models;
using Core.Models.DTOs.User;
using Mapster;

namespace Core.Mappings
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
