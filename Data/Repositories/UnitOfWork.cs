using Data.Persistence;
using Domain.Abstractions;

namespace Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
            => _context = context;

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
