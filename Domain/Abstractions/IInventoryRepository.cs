using Domain.Enums;

namespace Domain.Abstractions
{
    public interface IInventoryRepository : IRepository<Inventory, int>
    {
        Task<bool> ExistsByNameInMunicipalityAsync(string itemName, int municipalityId);
        Task<IEnumerable<Inventory>> GetByMunicipalityAsync(int municipalityId);
        Task<IEnumerable<Inventory>> GetByStatusAsync(InventoryStatus status);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int thresholdQuantity);
        Task<IEnumerable<Inventory>> GetAllOrderedByItemNameAsync();
    }
}
