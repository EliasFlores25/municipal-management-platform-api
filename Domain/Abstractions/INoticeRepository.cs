using Domain.Enums;

namespace Domain.Abstractions
{
    public interface INoticeRepository : IRepository<Notice, int>
    {
        Task<IEnumerable<Notice>> GetActiveByMunicipalityAsync(int municipalityId);
        Task<IEnumerable<Notice>> GetByCategoryAsync(NoticeCategory category);
        Task<IEnumerable<Notice>> GetActiveByCategoryAsync(NoticeCategory category);
        Task<IEnumerable<Notice>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
