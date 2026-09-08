
namespace FixHub.Application.Payment
{
    public interface IPaymentGateway
    {
        PaymentCheckoutUrls CheckoutUrls { get; }

        Task<PaymentCheckoutResult> CreateCheckoutAsync(
            PaymentCheckoutRequest request,
            CancellationToken cancellationToken);
    }

    public sealed record PaymentCheckoutUrls(string SuccessUrl, string ErrorUrl, string CancelUrl);
}
