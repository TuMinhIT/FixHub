using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Orders.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, List<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllOrdersHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.OrderRepository.GetAll()
                .Include(x => x.OrderDetails)
                .Include(x => x.Payment)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<OrderResponse>>(orders);
        }
    }
}
