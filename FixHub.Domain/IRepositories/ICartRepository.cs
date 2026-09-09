using FixHub.Domain.Entities;

namespace FixHub.Domain.IRepositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
