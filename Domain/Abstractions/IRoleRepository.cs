
namespace Domain.Abstractions
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllOrderedByNameAsync();
        Task<Role?> GetByIdAsync(int id);
    }
}
