
namespace Domain.Abstractions
{
    public interface IMunicipalityRepository : IRepository<Municipality, int>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Municipality>> GetAllOrderedByNameAsync();
    }
}
