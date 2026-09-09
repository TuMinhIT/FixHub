using FixHub.Application.Features.Orders.DTOs;
using MediatR;

namespace FixHub.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderResponse>
    {
        public Guid Id { get; set; }
        public GetOrderByIdQuery(Guid id) => Id = id;
    }
}
