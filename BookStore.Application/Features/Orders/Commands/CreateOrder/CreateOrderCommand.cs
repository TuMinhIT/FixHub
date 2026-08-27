using MediatR;

namespace FixHub.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }
        public Guid? AddressId { get; set; }
    }
}
