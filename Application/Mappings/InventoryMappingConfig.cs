using Domain;
using Mapster;
using static Application.DTOs.InventoryDtos;

namespace Application.Mappings
{
    public class InventoryMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Inventory, InventoryResponse>()
                .Map(dest => dest.State, src => src.State.ToString())
                .Map(dest => dest.MunicipalityName, src => src.Municipality != null ? src.Municipality.Name : "No Asignado");
        }
    }
}
