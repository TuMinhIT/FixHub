using BookStore.Domain.Entities;
using BookStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context): base(context)
        {
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
