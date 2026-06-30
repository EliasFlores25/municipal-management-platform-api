using Domain;
using Mapster;
using static Application.DTOs.ProblemDtos;

namespace Application.Mappings;

public class ProblemMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Problem, ProblemResponse>()
            .Map(dest => dest.Type, src => src.Type.ToString())
            .Map(dest => dest.Severity, src => src.Severity.ToString())
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.MunicipalityName, src => src.Municipality != null ? src.Municipality.Name : "No Asignado");
    }
}