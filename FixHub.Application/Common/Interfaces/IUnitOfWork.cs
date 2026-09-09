using FixHub.Domain.IRepositories;
using FixHub.Domain.Entities;

namespace FixHub.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IAddressRepository AddressRepository { get; }
        IKnowledgeArticleRepository KnowledgeArticleRepository { get; }
        IKnowledgeChunkRepository KnowledgeChunkRepository { get; }
        ICartRepository CartRepository { get; }
        IRepository<InventoryTransaction> InventoryTransactionRepository { get; }
        IRepository<StockReservation> StockReservationRepository { get; }
        IRepository<PaymentEvent> PaymentEventRepository { get; }
        IRepository<RagFeedback> RagFeedbackRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(
            CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(
            CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(
            CancellationToken cancellationToken = default);
    }
}
