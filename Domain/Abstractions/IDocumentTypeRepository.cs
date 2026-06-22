
namespace Domain.Abstractions
{
    public interface IDocumentTypeRepository : IRepository<DocumentType, int>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<DocumentType>> GetAllOrderedByNameAsync();
    }
}