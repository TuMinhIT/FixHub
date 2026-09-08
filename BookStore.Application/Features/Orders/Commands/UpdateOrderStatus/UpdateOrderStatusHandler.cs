using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FixHub.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderInventoryService _inventoryService;

        public UpdateOrderStatusHandler(IUnitOfWork unitOfWork, IOrderInventoryService inventoryService)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.FindById(request.Id);
            if (order == null) throw new NotFoundException(nameof(Order), request.Id);
            if (!OrderStatuses.CanTransition(order.Status, request.Status))
                throw new BadRequestException($"Cannot transition order from {order.Status} to {request.Status}.");
            
            var nextStatus = new[]
            {
                OrderStatuses.PendingPayment,
                OrderStatuses.Paid,
                OrderStatuses.Processing,
                OrderStatuses.Shipping,
                OrderStatuses.Completed,
                OrderStatuses.Cancelled,
                OrderStatuses.Refunded
            }.FirstOrDefault(x => string.Equals(x, request.Status, StringComparison.OrdinalIgnoreCase));
            if (nextStatus == null)
                throw new BadRequestException("Invalid order status.");
            if (!OrderStatuses.CanTransition(order.Status, nextStatus))
                throw new BadRequestException($"Cannot transition order from {order.Status} to {nextStatus}.");

            order.Status = nextStatus;
            if (nextStatus == OrderStatuses.Cancelled)
                await _inventoryService.ReleaseAsync(order.Id, "admin-cancelled", cancellationToken);
            else if (nextStatus == OrderStatuses.Paid)
                await _inventoryService.FinalizeAsync(order.Id, "payment-confirmed", cancellationToken);
            
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
