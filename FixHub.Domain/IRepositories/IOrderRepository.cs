using FixHub.Domain.Entities;

namespace FixHub.Domain.IRepositories
{
public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
}
