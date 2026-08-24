using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;

namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
    }
}
