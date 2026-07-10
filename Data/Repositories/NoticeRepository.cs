using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class NoticeRepository : INoticeRepository
    {
        private readonly ApplicationDbContext _context;

        public NoticeRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<IEnumerable<Notice>> GetActiveByMunicipalityAsync(int municipalityId)
        {
            if (municipalityId <= 0)
                throw new ArgumentException("El ID del municipio debe ser válido.", nameof(municipalityId));

            return await _context.Notices
                .AsNoTracking()
                .Where(n => n.MunicipalityId == municipalityId && !n.IsArchived)
                .OrderByDescending(n => n.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notice>> GetByCategoryAsync(NoticeCategory category)
        {
            return await _context.Notices
                .AsNoTracking()
                .Where(n => n.Category == category)
                .OrderByDescending(n => n.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notice>> GetActiveByCategoryAsync(NoticeCategory category)
        {
            return await _context.Notices
                .AsNoTracking()
                .Where(n => n.Category == category && !n.IsArchived)
                .OrderByDescending(n => n.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notice>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("La fecha de fin no puede ser menor a la fecha de inicio.");

            return await _context.Notices
                .AsNoTracking()
                .Where(n => n.RegistrationDate >= startDate && n.RegistrationDate <= endDate)
                .OrderByDescending(n => n.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notice>> GetAllAsync()
        {
            return await _context.Notices
                .AsNoTracking()
                .OrderByDescending(n => n.RegistrationDate)
                .ToListAsync();
        }

        public async Task<Notice?> GetByIdAsync(int id)
        {
            return await _context.Notices.FindAsync(id);
        }

        public async Task AddAsync(Notice entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.Notices.AddAsync(entity);
        }

        public Task UpdateAsync(Notice entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Notices.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Notice entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Notices.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
