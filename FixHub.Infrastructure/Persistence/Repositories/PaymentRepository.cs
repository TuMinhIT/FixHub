
using FixHub.Domain.IRepositories;
using PaymentEntity = FixHub.Domain.Entities.Payment;

namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : Repository<PaymentEntity>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
