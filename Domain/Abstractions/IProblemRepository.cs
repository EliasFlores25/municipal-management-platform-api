using Domain.Enums;

namespace Domain.Abstractions
{
    public interface IProblemRepository : IRepository<Problem, int>
    {
        Task<IEnumerable<Problem>> GetByMunicipalityAsync(int municipalityId);
        Task<IEnumerable<Problem>> GetByMunicipalityAndStatusAsync(int municipalityId, ProblemStatus status);
        Task<IEnumerable<Problem>> GetBySeverityAndStatusAsync(ProblemSeverity severity, ProblemStatus status);
        Task<IEnumerable<Problem>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
