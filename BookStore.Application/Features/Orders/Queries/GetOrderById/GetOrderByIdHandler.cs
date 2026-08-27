using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Orders.DTOs;
using MediatR;

namespace FixHub.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.FindById(request.Id);
            if (order == null) throw new Exception("Order not found");
            return _mapper.Map<OrderResponse>(order);
        }
    }
}
