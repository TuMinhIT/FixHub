
namespace FixHub.Infrastructure.Payment
{
    public sealed class SePayOptions
    {
        public const string SectionName = "SePay";

        public string MerchantId { get; set; } = null!;

        public string SecretKey { get; set; } = null!;

        public string Currency { get; set; } = null;

        public string CheckoutUrl { get; set; } = null!;
    }
}
