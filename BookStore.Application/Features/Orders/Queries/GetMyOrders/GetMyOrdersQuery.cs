using FixHub.Application.Features.Orders.DTOs;
using MediatR;

namespace FixHub.Application.Features.Orders.Queries.GetMyOrders
{
    public class GetMyOrdersQuery : IRequest<List<OrderResponse>>
    {
        public Guid UserId { get; set; }
        public GetMyOrdersQuery(Guid userId) => UserId = userId;
    }
}
