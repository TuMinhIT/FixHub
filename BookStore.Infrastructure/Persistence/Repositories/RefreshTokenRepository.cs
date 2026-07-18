using BookStore.Domain.Entities;
using BookStore.Domain.IRepositories;
using System;
using System.Linq.Expressions;

namespace BookStore.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public bool RevokeToken(string refreshToken)
        {
            var token = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (token != null)
            {
                token.RevokedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return true;
            }
            return false;
        } 
    }
}
