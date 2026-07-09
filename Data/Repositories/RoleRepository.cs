using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
            => _context = context;

        public async Task AddAsync(Role role)
        {
            role = role ?? throw new ArgumentNullException(nameof(role));
            await _context.Roles.AddAsync(role);
        }

        public async Task<bool> ExistsByRoleNameAsync(string name)
        {
            if (name == null) throw new ArgumentException("El nombre no puede estar vacío", nameof(name));
            var normalizedName = name.ToUpperInvariant();
            return await _context.Roles.AnyAsync(r => r.Name == normalizedName);
        }

        public async Task<IEnumerable<Role>> GetAllOrderedByNameAsync()
            => await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();

        public async Task<Role?> GetByIdAsync(int id)
            => await _context.Roles.FindAsync(id);

    }
}
