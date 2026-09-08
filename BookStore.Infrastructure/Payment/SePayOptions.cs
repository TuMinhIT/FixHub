
namespace FixHub.Infrastructure.Payment
{
    public sealed class SePayOptions
    {
        public const string SectionName = "SePay";

        public string MerchantId { get; set; } = null!;

        public string SecretKey { get; set; } = null!;

        public string Currency { get; set; } = string.Empty;

        public string CheckoutUrl { get; set; } = null!;
        public string SuccessUrl { get; set; } = "http://localhost:5173/orders";
        public string ErrorUrl { get; set; } = "http://localhost:5173/orders/error";
        public string CancelUrl { get; set; } = "http://localhost:5173/orders/cancel";
    }
}
