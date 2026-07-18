using BookStore.Domain.Entities;
using BookStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace BookStore.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.Include(x=>x.User).FirstOrDefaultAsync(rt =>rt.Token == token);
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
