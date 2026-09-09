using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;

namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class ServiceRepository : Repository<RepairService>, IServiceRepository
    {
        public ServiceRepository(AppDbContext context) : base(context)
        {
        }
    }
}
