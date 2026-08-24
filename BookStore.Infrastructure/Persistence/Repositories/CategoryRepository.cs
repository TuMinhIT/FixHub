using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;

namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
