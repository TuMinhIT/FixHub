using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Orders.Services;

public sealed class OrderInventoryService : IOrderInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderInventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ReleaseAsync(Guid orderId, string reference, CancellationToken cancellationToken = default)
    {
        var reservations = await _unitOfWork.StockReservationRepository
            .Find(x => x.OrderId == orderId && x.ReleasedAt == null && x.FinalizedAt == null)
            .ToListAsync(cancellationToken);
        if (reservations.Count == 0)
            return;

        var productIds = reservations.Select(x => x.ProductId).Distinct().ToList();
        var products = await _unitOfWork.ProductRepository
            .Find(x => productIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
        var productById = products.ToDictionary(x => x.Id);

        foreach (var reservation in reservations)
        {
            if (!productById.TryGetValue(reservation.ProductId, out var product))
                continue;

            product.StockQuantity += reservation.Quantity;
            reservation.ReleasedAt = DateTime.UtcNow;
            await _unitOfWork.InventoryTransactionRepository.AddAsync(new InventoryTransaction
            {
                ProductId = product.Id,
                OrderId = orderId,
                Type = InventoryTransactionType.Released,
                Quantity = reservation.Quantity,
                Reference = reference
            });
        }
    }

    public async Task FinalizeAsync(Guid orderId, string reference, CancellationToken cancellationToken = default)
    {
        var reservations = await _unitOfWork.StockReservationRepository
            .Find(x => x.OrderId == orderId && x.ReleasedAt == null && x.FinalizedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var reservation in reservations)
        {
            reservation.FinalizedAt = DateTime.UtcNow;
            await _unitOfWork.InventoryTransactionRepository.AddAsync(new InventoryTransaction
            {
                ProductId = reservation.ProductId,
                OrderId = orderId,
                Type = InventoryTransactionType.Sold,
                Quantity = reservation.Quantity,
                Reference = reference
            });
        }
    }
}
