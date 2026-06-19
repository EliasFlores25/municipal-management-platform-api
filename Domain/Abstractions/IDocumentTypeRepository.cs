
namespace Domain.Abstractions
{
    public interface IDocumentTypeRepository : IRepository<Document, int>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<DocumentType>> GetAllOrderedByNameAsync();
    }
}