using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProblemRepository : IProblemRepository
    {
        private readonly ApplicationDbContext _context;

        public ProblemRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<IEnumerable<Problem>> GetByMunicipalityAsync(int municipalityId)
        {
            if (municipalityId <= 0)
                throw new ArgumentException("El ID del municipio debe ser válido.", nameof(municipalityId));

            return await _context.Problems
                .AsNoTracking()
                .Where(p => p.MunicipalityId == municipalityId)
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Problem>> GetByMunicipalityAndStatusAsync(int municipalityId, ProblemStatus status)
        {
            if (municipalityId <= 0)
                throw new ArgumentException("El ID del municipio debe ser válido.", nameof(municipalityId));

            return await _context.Problems
                .AsNoTracking()
                .Where(p => p.MunicipalityId == municipalityId && p.Status == status)
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Problem>> GetBySeverityAndStatusAsync(ProblemSeverity severity, ProblemStatus status)
        {
            return await _context.Problems
                .AsNoTracking()
                .Where(p => p.Severity == severity && p.Status == status)
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Problem>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("La fecha de fin no puede ser menor a la fecha de inicio.");

            return await _context.Problems
                .AsNoTracking()
                .Where(p => p.RegistrationDate >= startDate && p.RegistrationDate <= endDate)
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Problem>> GetAllAsync()
        {
            return await _context.Problems
                .AsNoTracking()
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();
        }

        public async Task<Problem?> GetByIdAsync(int id)
        {
            return await _context.Problems.FindAsync(id);
        }

        public async Task AddAsync(Problem entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.Problems.AddAsync(entity);
        }

        public Task UpdateAsync(Problem entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Problems.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Problem entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Problems.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
