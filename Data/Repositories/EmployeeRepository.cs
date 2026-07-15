using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("El código no puede estar vacío.", nameof(code));

            var normalizedCode = code.Trim().ToUpper();
            return await _context.Employees.AnyAsync(e => e.Code == normalizedCode);
        }

        public async Task<Employee?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("El código no puede estar vacío.", nameof(code));

            var normalizedCode = code.Trim().ToUpper();

            return await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Municipality)
                .FirstOrDefaultAsync(e => e.Code == normalizedCode);
        }

        public async Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Position)
                .Where(e => e.State == status)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByPositionAsync(int positionId)
        {
            if (positionId <= 0)
                throw new ArgumentException("El ID del cargo debe ser válido.", nameof(positionId));

            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.PositionId == positionId)
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByMunicipalityAsync(int municipalityId)
        {
            if (municipalityId <= 0)
                throw new ArgumentException("El ID del municipio debe ser válido.", nameof(municipalityId));

            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Position)
                .Where(e => e.MunicipalityId == municipalityId)
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Position)
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task AddAsync(Employee entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.Employees.AddAsync(entity);
        }

        public Task UpdateAsync(Employee entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Employees.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Employee entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Employees.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
