using Domain;
using Mapster;
using static Application.DTOs.EmployeeDtos;

namespace Application.Mappings
{
    public class EmployeeMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Employee, EmployeeResponse>()
                .Map(dest => dest.State, src => src.State.ToString())
                .Map(dest => dest.PositionName, src => src.Position != null ? src.Position.Name : "Sin Cargo")
                .Map(dest => dest.MunicipalityName, src => src.Municipality != null ? src.Municipality.Name : "Sin Municipio");
        }
    }
}
