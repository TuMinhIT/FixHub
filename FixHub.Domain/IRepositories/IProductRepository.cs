using FixHub.Domain.Entities;

namespace FixHub.Domain.IRepositories
{
public interface IProductRepository : IRepository<Product>
{
    Task<bool> TryReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}
}
