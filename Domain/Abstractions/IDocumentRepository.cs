using Domain.Enums;

namespace Domain.Abstractions
{
    public interface IDocumentRepository : IRepository<Document, int>
    {
        Task<bool> ExistsByDocumentNumberAsync(string documentNumber);
        Task<Document?> GetByDocumentNumberAsync(string documentNumber);
        Task<IEnumerable<Document>> GetByMunicipalityAndStatusAsync(int municipalityId, DocumentStatus status);
        Task<IEnumerable<Document>> GetByDocumentTypeAsync(int documentTypeId);
        Task<IEnumerable<Document>> GetByEmissionDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}