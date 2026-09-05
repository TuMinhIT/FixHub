using FixHub.Application.Common.Interfaces;
using FixHub.Domain.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FixHub.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IServiceRepository serviceRepository,
            IOrderRepository orderRepository,
            IAddressRepository addressRepository,
            IPaymentRepository paymentRepository,
            IKnowledgeArticleRepository knowledgeArticleRepository)
        {
            _context = context;
            UserRepository = userRepository;
            RefreshTokenRepository = refreshTokenRepository;
            ProductRepository = productRepository;
            CategoryRepository = categoryRepository;
            PaymentRepository = paymentRepository;
            ServiceRepository = serviceRepository;
            OrderRepository = orderRepository;
            AddressRepository = addressRepository;
            KnowledgeArticleRepository = knowledgeArticleRepository;
        }

        public IUserRepository UserRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IServiceRepository ServiceRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IAddressRepository AddressRepository { get; }
        public IKnowledgeArticleRepository KnowledgeArticleRepository { get; }
        public IPaymentRepository PaymentRepository { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                return;

            try
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                return;

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}