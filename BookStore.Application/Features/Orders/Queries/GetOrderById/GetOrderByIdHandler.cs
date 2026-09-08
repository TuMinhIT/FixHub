using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Features.Orders.DTOs;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetDetailsByIdAsync(request.Id, cancellationToken);
            if (order == null) throw new NotFoundException(nameof(Order), request.Id);
            if (!string.Equals(_currentUserService.Role, "Admin", StringComparison.OrdinalIgnoreCase)
                && order.UserId != _currentUserService.UserId)
                throw new ForbiddenException("You do not have access to this order.");
            return _mapper.Map<OrderResponse>(order);
        }
    }
}
