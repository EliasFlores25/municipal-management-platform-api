using Domain;
using Mapster;
using static Application.DTOs.ProjectDtos;

namespace Application.Mappings
{
    public class ProjectMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Project, ProjectResponse>()
                .Map(dest => dest.State, src => src.State.ToString())
                .Map(dest => dest.MunicipalityName, src => src.Municipality != null ? src.Municipality.Name : "No Asignado");
        }
    }
}
