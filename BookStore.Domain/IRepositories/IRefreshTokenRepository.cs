using BookStore.Domain.Entities;


namespace BookStore.Domain.IRepositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        public bool RevokeToken(string refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);

    }
}
