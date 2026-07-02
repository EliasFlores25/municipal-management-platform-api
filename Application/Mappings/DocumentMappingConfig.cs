using Domain;
using Mapster;
using static Application.DTOs.DocumentDtos;

namespace Application.Mappings
{
    public class DocumentMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Document, DocumentResponse>()
                .Map(dest => dest.State, src => src.State.ToString())
                .Map(dest => dest.DocumentTypeName, src => src.DocumentType != null ? src.DocumentType.Name : "Sin Especificar")
                .Map(dest => dest.MunicipalityName, src => src.Municipality != null ? src.Municipality.Name : "Sin Municipio");
        }
    }
}
