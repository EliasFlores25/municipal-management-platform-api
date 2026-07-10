using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<IEnumerable<Project>> GetByMunicipalityAsync(int municipalityId)
        {
            if (municipalityId <= 0)
                throw new ArgumentException("El ID del municipio debe ser válido.", nameof(municipalityId));

            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.MunicipalityId == municipalityId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetByMunicipalityAndStatusAsync(int municipalityId, ProjectStatus state)
        {
            if (municipalityId <= 0)
                throw new ArgumentException("El ID del municipio debe ser válido.", nameof(municipalityId));

            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.MunicipalityId == municipalityId && p.State == state)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsWithHighBudgetAsync(decimal minBudget)
        {
            if (minBudget < 0)
                throw new ArgumentException("El presupuesto mínimo de búsqueda no puede ser negativo.", nameof(minBudget));

            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.Budget >= minBudget)
                .OrderByDescending(p => p.Budget)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetOverdueProjectsAsync(DateTime referenceDate)
        {
            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.EndDate < referenceDate
                         && p.State != ProjectStatus.Finalizado
                         && p.State != ProjectStatus.Cancelado)
                .OrderBy(p => p.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetAllOrderByNameAsync()
        {
            return await _context.Projects
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task AddAsync(Project entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.Projects.AddAsync(entity);
        }

        public Task UpdateAsync(Project entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Projects.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Project entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Projects.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
