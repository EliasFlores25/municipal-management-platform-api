using Domain.Enums;

namespace Domain.Abstractions
{
    public interface IProjectRepository : IRepository<Project, int>
    {
        Task<IEnumerable<Project>> GetByMunicipalityAsync(int municipalityId);
        Task<IEnumerable<Project>> GetByMunicipalityAndStatusAsync(int municipalityId, ProjectStatus state);
        Task<IEnumerable<Project>> GetProjectsWithHighBudgetAsync(decimal minBudget);
        Task<IEnumerable<Project>> GetOverdueProjectsAsync(DateTime referenceDate);
        Task<IEnumerable<Project>> GetAllOrderByNameAsync();
    }
}
