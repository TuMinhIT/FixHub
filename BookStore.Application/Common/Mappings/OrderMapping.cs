using AutoMapper;
using FixHub.Application.Features.Orders.Commands.CreateOrder;
using FixHub.Application.Features.Orders.DTOs;
using FixHub.Domain.Entities;
using PaymentEntity = FixHub.Domain.Entities.Payment;

namespace FixHub.Application.Common.Mappings
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Order, OrderResponse>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderDetails));
            CreateMap<OrderDetail, OrderItemResponse>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.ProductNameSnapshot ?? s.ServiceNameSnapshot))
                .ForMember(d => d.Sku, o => o.MapFrom(s => s.SkuSnapshot));
            CreateMap<PaymentEntity, PaymentSummary>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Method, o => o.MapFrom(s => s.Method.ToString()));
            CreateMap<CreateOrderCommand, Order>();
        }
    }
}
