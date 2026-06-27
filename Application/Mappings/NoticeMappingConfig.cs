using Domain;
using Mapster;
using static Application.DTOs.NoticeDtos;

namespace Application.Mappings;

public class NoticeMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Notice, NoticeResponse>()
            .Map(dest => dest.Category, src => src.Category.ToString())
            .Map(dest => dest.MunicipalityName, src => src.Municipality != null ? src.Municipality.Name : "No Asignado");
    }
}