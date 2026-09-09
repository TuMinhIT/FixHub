using FixHub.Application.Payment;
using Microsoft.Extensions.Options;

namespace FixHub.Infrastructure.Payment
{

    public sealed class SePayGateway : IPaymentGateway
    {
        private readonly SePayOptions _options;

        public PaymentCheckoutUrls CheckoutUrls => new(
            _options.SuccessUrl,
            _options.ErrorUrl,
            _options.CancelUrl);

        public SePayGateway(
            IOptions<SePayOptions> options)
        {
            _options = options.Value;
        }

        public Task<PaymentCheckoutResult> CreateCheckoutAsync(
            PaymentCheckoutRequest request,
            CancellationToken cancellationToken)
        {
            var fields = new Dictionary<string, string>
            {
                ["order_amount"] =
                    ((long)request.Amount).ToString(),

                ["merchant"] =
                    _options.MerchantId,

                ["currency"] =_options.Currency,
                   
                ["operation"] =
                    "PURCHASE",

                ["order_description"] =
                    request.Description,

                ["order_invoice_number"] =
                    request.InvoiceNumber,

                ["payment_method"] =
                    "BANK_TRANSFER",

                ["success_url"] =
                    request.SuccessUrl,

                ["error_url"] =
                    request.ErrorUrl,

                ["cancel_url"] =
                    request.CancelUrl
            };

            var signature =
                SePaySignatureService.Generate(
                    fields,
                    _options.SecretKey);

            fields["signature"] = signature;

            return Task.FromResult(
                new PaymentCheckoutResult(
                    _options.CheckoutUrl,
                    fields));
        }
    }
}
