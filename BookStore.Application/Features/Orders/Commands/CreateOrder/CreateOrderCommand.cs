using MediatR;

namespace FixHub.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<CreateOrderResponse>
    {
        public Guid UserId { get; set; }
        public Guid? AddressId { get; set; }
        public string? Note { get; set; }
        public string PaymentMethod { get; set; } = "sepay";
        public string? IdempotencyKey { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }

    public class OrderItemRequest
    {
        public Guid? ProductId { get; set; }
        public Guid? RepairServiceId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }
}