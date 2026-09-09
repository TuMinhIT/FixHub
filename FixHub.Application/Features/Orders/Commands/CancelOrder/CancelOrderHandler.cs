using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrderInventoryService _inventoryService;

    public CancelOrderHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IOrderInventoryService inventoryService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _inventoryService = inventoryService;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetDetailsByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);
        if (order.UserId != _currentUserService.UserId
            && !string.Equals(_currentUserService.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("You do not have access to this order.");
        if (!OrderStatuses.CanTransition(order.Status, OrderStatuses.Cancelled))
            throw new BadRequestException($"Order cannot be cancelled from status {order.Status}.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            order.Status = OrderStatuses.Cancelled;
            if (order.Payment != null)
                order.Payment.MarkAsCancelled();
            await _inventoryService.ReleaseAsync(order.Id, "order-cancelled", cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return true;
    }
}
