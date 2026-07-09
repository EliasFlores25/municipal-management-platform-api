using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class MunicipalityRepository : IMunicipalityRepository
    {
        private readonly ApplicationDbContext _context;
        public MunicipalityRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (name == null) throw new ArgumentException("El nombre no puede estar vacío", nameof(name));
            var normalizedName = name.ToUpperInvariant();
            return await _context.Municipalities
                .AnyAsync(m => m.Name.ToUpper() == normalizedName);
        }

        public async Task<IEnumerable<Municipality>> GetAllOrderedByNameAsync()
            => await _context.Municipalities
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ToListAsync();

        public async Task<Municipality?> GetByIdAsync(int id)
            => await _context.Municipalities.FindAsync(id);

        public async Task AddAsync(Municipality entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.Municipalities.AddAsync(entity);
        }

        public Task UpdateAsync(Municipality entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Municipalities.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Municipality entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Municipalities.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
