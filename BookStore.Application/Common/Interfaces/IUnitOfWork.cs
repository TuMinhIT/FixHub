using FixHub.Domain.IRepositories;

namespace FixHub.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IOrderRepository OrderRepository { get; }
        IKnowledgeArticleRepository KnowledgeArticleRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
