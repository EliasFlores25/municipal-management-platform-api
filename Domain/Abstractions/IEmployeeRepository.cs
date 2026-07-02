using Domain.Enums;

namespace Domain.Abstractions
{
    public interface IEmployeeRepository : IRepository<Employee, int>
    {
        Task<bool> ExistsByCodeAsync(string code);
        Task<Employee?> GetByCodeAsync(string code);
        Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status);
        Task<IEnumerable<Employee>> GetByPositionAsync(int positionId);
        Task<IEnumerable<Employee>> GetByMunicipalityAsync(int municipalityId);
        Task<IEnumerable<Employee>> GetAllAsync();
    }
}