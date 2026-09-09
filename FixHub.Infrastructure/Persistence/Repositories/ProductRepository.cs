using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> TryReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            if (quantity <= 0)
                return false;

            var updated = await _context.Products
                .Where(x => x.Id == productId && x.IsActive && x.StockQuantity >= quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.StockQuantity, x => x.StockQuantity - quantity), cancellationToken);
            return updated == 1;
        }
    }
}
