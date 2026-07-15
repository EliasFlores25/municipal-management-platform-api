using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("El número de documento no puede estar vacío.", nameof(documentNumber));

            var normalizedNumber = documentNumber.Trim().ToUpper();
            return await _context.Documents.AnyAsync(d => d.DocumentNumber == normalizedNumber);
        }

        public async Task<Document?> GetByDocumentNumberAsync(string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("El número de documento no puede estar vacío.", nameof(documentNumber));

            var normalizedNumber = documentNumber.Trim().ToUpper();

            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Municipality)
                .FirstOrDefaultAsync(d => d.DocumentNumber == normalizedNumber);
        }

        public async Task<IEnumerable<Document>> GetByMunicipalityAndStatusAsync(int municipalityId, DocumentStatus status)
        {
            if (municipalityId <= 0)
                throw new ArgumentException("El ID del municipio debe ser válido.", nameof(municipalityId));

            return await _context.Documents
                .AsNoTracking()
                .Include(d => d.DocumentType)
                .Where(d => d.MunicipalityId == municipalityId && d.State == status)
                .OrderByDescending(d => d.EmissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetByDocumentTypeAsync(int documentTypeId)
        {
            if (documentTypeId <= 0)
                throw new ArgumentException("El ID del tipo de documento debe ser válido.", nameof(documentTypeId));

            return await _context.Documents
                .AsNoTracking()
                .Where(d => d.DocumentTypeId == documentTypeId)
                .OrderByDescending(d => d.EmissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetByEmissionDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("La fecha de fin no puede ser menor a la fecha de inicio.");

            return await _context.Documents
                .AsNoTracking()
                .Include(d => d.DocumentType)
                .Where(d => d.EmissionDate >= startDate && d.EmissionDate <= endDate)
                .OrderByDescending(d => d.EmissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
            => await _context.Documents
                .AsNoTracking()
                .Include(d => d.DocumentType)
                .OrderByDescending(d => d.EmissionDate)
                .ToListAsync();

        public async Task<Document?> GetByIdAsync(int id)
            => await _context.Documents.FindAsync(id);

        public async Task AddAsync(Document entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.Documents.AddAsync(entity);
        }

        public Task UpdateAsync(Document entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Documents.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Document entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Documents.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
