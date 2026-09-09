using FixHub.Application.Features.Orders.DTOs;
using MediatR;

namespace FixHub.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<List<OrderResponse>>
    {
    }
}
