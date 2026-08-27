using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Orders.Queries.GetMyOrders
{
    public class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, List<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMyOrdersHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<OrderResponse>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.OrderRepository.Find(x => x.UserId == request.UserId).ToListAsync(cancellationToken);
            return _mapper.Map<List<OrderResponse>>(orders);
        }
    }
}
