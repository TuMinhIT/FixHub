using FixHub.Application.Common.Interfaces;
using FixHub.Domain.IRepositories;
using FixHub.Infrastructure.Persistence.Repositories;

namespace FixHub.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context, 
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IServiceRepository serviceRepository,
            IOrderRepository orderRepository,
            IKnowledgeArticleRepository knowledgeArticleRepository)
        {
            _context = context;
            UserRepository = userRepository;
            RefreshTokenRepository = refreshTokenRepository;
            ProductRepository = productRepository;
            CategoryRepository = categoryRepository;
            ServiceRepository = serviceRepository;
            OrderRepository = orderRepository;
            KnowledgeArticleRepository = knowledgeArticleRepository;
        }

        public IUserRepository UserRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IServiceRepository ServiceRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IKnowledgeArticleRepository KnowledgeArticleRepository { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}