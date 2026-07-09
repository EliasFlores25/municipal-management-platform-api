using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly ApplicationDbContext _context;

        public PositionRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (name == null) throw new ArgumentException("El nombre no puede estar vacío", nameof(name));
            return await _context.Positions
                .AnyAsync(p => p.Name == name);
        }

        public async Task<IEnumerable<Position>> GetAllOrderedByNameAsync()
            => await _context.Positions
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();

        public async Task<IEnumerable<Position>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Position>();

            var normalizedSearch = searchTerm.ToUpperInvariant();

            return await _context.Positions
                .AsNoTracking()
                .Where(p => p.Name.ToUpper().Contains(normalizedSearch))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Position?> GetByIdAsync(int id)
            => await _context.Positions.FindAsync(id);

        public async Task AddAsync(Position entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.Positions.AddAsync(entity);
        }

        public Task UpdateAsync(Position entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Positions.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Position entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Positions.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
