using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentTypeRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre no puede estar vacío.", nameof(name));

            return await _context.DocumentTypes.AnyAsync(dt => dt.Name == name);
        }

        public async Task<IEnumerable<DocumentType>> GetAllOrderedByNameAsync()
            => await _context.DocumentTypes
                .AsNoTracking()
                .OrderBy(dt => dt.Name)
                .ToListAsync();

        public async Task<DocumentType?> GetByIdAsync(int id)
            => await _context.DocumentTypes.FindAsync(id);

        public async Task AddAsync(DocumentType entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.DocumentTypes.AddAsync(entity);
        }

        public Task UpdateAsync(DocumentType entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.DocumentTypes.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(DocumentType entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.DocumentTypes.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
