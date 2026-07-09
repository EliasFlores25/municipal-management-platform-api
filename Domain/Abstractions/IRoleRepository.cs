
namespace Domain.Abstractions
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role);
        Task<bool> ExistsByRoleNameAsync(string name);
        Task<IEnumerable<Role>> GetAllOrderedByNameAsync();
        Task<Role?> GetByIdAsync(int id);
    }
}
