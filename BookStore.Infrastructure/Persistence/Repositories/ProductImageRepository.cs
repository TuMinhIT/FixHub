using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;

namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
