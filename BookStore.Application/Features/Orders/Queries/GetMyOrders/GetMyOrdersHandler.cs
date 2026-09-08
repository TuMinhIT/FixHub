using AutoMapper;
using FixHub.Application.Features.Orders.DTOs;
using FixHub.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Orders.Queries.GetMyOrders
{
    public class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, List<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetMyOrdersHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<List<OrderResponse>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.OrderRepository.Find(x => x.UserId == _currentUserService.UserId)
                .Include(x => x.OrderDetails)
                .Include(x => x.Payment)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<OrderResponse>>(orders);
        }
    }
}
