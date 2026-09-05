using MediatR;

namespace FixHub.Application.Features.Orders.Commands.CreateOrder
{

    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }
        public Guid? PaymentId { get; set; }
        public string CheckoutUrl { get; set; } = string.Empty;
        public IReadOnlyDictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
    }
}