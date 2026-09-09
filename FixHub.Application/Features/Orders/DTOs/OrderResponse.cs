namespace FixHub.Application.Features.Orders.DTOs
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public Guid? AddressId { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public IReadOnlyList<OrderItemResponse> Items { get; set; } = Array.Empty<OrderItemResponse>();
        public PaymentSummary? Payment { get; set; }
    }

    public sealed class OrderItemResponse
    {
        public Guid Id { get; init; }
        public Guid? ProductId { get; init; }
        public Guid? RepairServiceId { get; init; }
        public string? Name { get; init; }
        public string? Sku { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal Subtotal { get; init; }
    }

    public sealed class PaymentSummary
    {
        public Guid Id { get; init; }
        public string Status { get; init; } = string.Empty;
        public string Method { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string InvoiceNumber { get; init; } = string.Empty;
        public DateTime? PaidAt { get; init; }
    }
}
