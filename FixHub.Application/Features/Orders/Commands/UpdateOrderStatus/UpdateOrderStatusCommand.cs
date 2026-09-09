using MediatR;

namespace FixHub.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
