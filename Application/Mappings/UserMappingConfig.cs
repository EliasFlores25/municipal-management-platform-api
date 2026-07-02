using Domain;
using Mapster;
using static Application.DTOs.UserDtos;

namespace Application.Mappings
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserResponse>()
                .Map(dest => dest.RoleName, src => src.Role != null ? src.Role.Name : "Sin Rol");
        }
    }
}
