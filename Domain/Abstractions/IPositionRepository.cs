
namespace Domain.Abstractions
{
    public interface IPositionRepository : IRepository<Position, int>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Position>> GetAllOrderedByNameAsync();
        Task<IEnumerable<Position>> SearchByNameAsync(string searchTerm);
    }
}
