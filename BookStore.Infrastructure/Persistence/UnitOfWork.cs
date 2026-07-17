using BookStore.Application.Common.Interfaces;
using BookStore.Domain.IRepositories;

namespace BookStore.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;

        public UnitOfWork(AppDbContext context, IUserRepository userRepository, IBookRepository bookRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
        }

        public IUserRepository UserRepository => _userRepository;
        public IBookRepository BookRepository => _bookRepository;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}