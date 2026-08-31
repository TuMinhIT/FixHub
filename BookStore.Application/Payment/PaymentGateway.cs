
namespace FixHub.Application.Payment
{
    public interface IPaymentGateway
    {
        Task<PaymentCheckoutResult> CreateCheckoutAsync(
            PaymentCheckoutRequest request,
            CancellationToken cancellationToken);
    }
}
