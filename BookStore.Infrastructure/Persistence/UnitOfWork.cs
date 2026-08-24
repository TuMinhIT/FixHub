using FixHub.Application.Common.Interfaces;
using FixHub.Domain.IRepositories;

namespace FixHub.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UnitOfWork(AppDbContext context, IUserRepository userRepository,
            IBookRepository bookRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public IUserRepository UserRepository => _userRepository;
        public IBookRepository BookRepository => _bookRepository;
        public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}