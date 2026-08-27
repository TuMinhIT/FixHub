using AutoMapper;
using FixHub.Application.Features.Orders.Commands.CreateOrder;
using FixHub.Application.Features.Orders.DTOs;
using FixHub.Domain.Entities;

namespace FixHub.Application.Common.Mappings
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Order, OrderResponse>();
            CreateMap<CreateOrderCommand, Order>();
        }
    }
}
