using Data.Persistence;
using Domain;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(int roleId)
        {
            if (roleId <= 0)
                throw new ArgumentException("El ID del rol debe ser válido.", nameof(roleId));

            return await _context.Users
                .AsNoTracking()
                .Where(u => u.RoleId == roleId)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByStatusAsync(bool isActive)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive == isActive)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El correo electrónico no puede estar vacío.", nameof(email));

            var normalizedEmail = email.Trim().ToLower();

            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El correo electrónico no puede estar vacío.", nameof(email));

            var normalizedEmail = email.Trim().ToLower();

            return await _context.Users.AnyAsync(u => u.Email == normalizedEmail);
        }

        public async Task AddAsync(User user)
        {
            user = user ?? throw new ArgumentNullException(nameof(user));
            await _context.Users.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            user = user ?? throw new ArgumentNullException(nameof(user));
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
