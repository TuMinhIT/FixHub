using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;

namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
