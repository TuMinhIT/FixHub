using FixHub.Domain.Entities;

namespace FixHub.Domain.IRepositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<List<Address>> GetByUserIdAsync(Guid userId);
    }
}
