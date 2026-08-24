using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context): base(context)
        {
            _context = context;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(x => x.Email == email);
        }
    }
}
