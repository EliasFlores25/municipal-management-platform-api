using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
            => _context = context;


        public async Task<bool> ExistsByNameInMunicipalityAsync(string itemName, int municipalityId)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                throw new ArgumentException("El nombre del ítem no puede estar vacío.", nameof(itemName));

            if (municipalityId <= 0)
                throw new ArgumentException("El ID del municipio debe ser válido.", nameof(municipalityId));

            return await _context.Inventories
                .AnyAsync(i => i.ItemName == itemName && i.MunicipalityId == municipalityId);
        }

        public async Task<IEnumerable<Inventory>> GetByMunicipalityAsync(int municipalityId)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Municipality)
                .Where(i => i.MunicipalityId == municipalityId)
                .OrderBy(i => i.ItemName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetByStatusAsync(InventoryStatus status)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Where(i => i.State == status)
                .OrderBy(i => i.ItemName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int thresholdQuantity)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Where(i => i.Quantity <= thresholdQuantity && i.State != InventoryStatus.Baja)
                .OrderBy(i => i.Quantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetAllOrderedByItemNameAsync()
        {
            return await _context.Inventories
                .AsNoTracking()
                .OrderBy(i => i.ItemName)
                .ToListAsync();
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Inventories.FindAsync(id);
        }

        public async Task AddAsync(Inventory entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.Inventories.AddAsync(entity);
        }

        public Task UpdateAsync(Inventory entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Inventories.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Inventory entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Inventories.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
