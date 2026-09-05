using System.Collections.Generic;
using System.Text.Json;

namespace FixHub.Domain.Entities
{
    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5
    }

    public enum PaymentMethod
    {
        SePay = 1
    }

    public class Payment
    {
        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public decimal Amount { get; private set; }

        public PaymentMethod Method { get; private set; }

        public PaymentStatus Status { get; private set; }

        public string InvoiceNumber { get; private set; } = null!;

        public string? GatewayTransactionId { get; private set; }

        public string? IdempotencyKey { get; private set; }

        public string? CheckoutUrl { get; private set; }

        public string? CheckoutFieldsJson { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? PaidAt { get; private set; }

        private Payment()
        {
        }

        public static Payment Create(
            Guid orderId,
            decimal amount,
            string invoiceNumber,
            string? idempotencyKey = null)
        {
            return new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Amount = amount,
                InvoiceNumber = invoiceNumber,
                Method = PaymentMethod.SePay,
                Status = PaymentStatus.Pending,
                IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey)
                    ? Guid.NewGuid().ToString("N")
                    : idempotencyKey.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsPaid(string transactionId)
        {
            if (Status == PaymentStatus.Success)
                return;

            GatewayTransactionId = transactionId;
            Status = PaymentStatus.Success;
            PaidAt = DateTime.UtcNow;
        }

        public void SetCheckoutData(string checkoutUrl, IReadOnlyDictionary<string, string> fields)
        {
            CheckoutUrl = checkoutUrl;
            CheckoutFieldsJson = JsonSerializer.Serialize(fields ?? new Dictionary<string, string>());
        }

        public IReadOnlyDictionary<string, string> GetCheckoutFields()
        {
            if (string.IsNullOrWhiteSpace(CheckoutFieldsJson))
                return new Dictionary<string, string>();

            try
            {
                var values = JsonSerializer.Deserialize<Dictionary<string, string>>(CheckoutFieldsJson);
                return values ?? new Dictionary<string, string>();
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
