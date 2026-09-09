namespace FixHub.Application.Payment.Queries.GetPayment;

public sealed class PaymentResponse
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public string Method { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string InvoiceNumber { get; init; } = string.Empty;
    public string? GatewayTransactionId { get; init; }
    public string? CheckoutUrl { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? PaidAt { get; init; }
}
