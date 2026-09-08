using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FixHub.Infrastructure.BackgroundJobs;

public sealed class StockReservationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StockReservationCleanupService> _logger;

    public StockReservationCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<StockReservationCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ReleaseExpiredReservationsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to release expired stock reservations.");
            }
        }
    }

    private async Task ReleaseExpiredReservationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var inventoryService = scope.ServiceProvider.GetRequiredService<IOrderInventoryService>();

        var orderIds = await unitOfWork.StockReservationRepository.GetAll()
            .Where(x => x.ExpiresAt <= DateTime.UtcNow && x.ReleasedAt == null && x.FinalizedAt == null)
            .Select(x => x.OrderId)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var orderId in orderIds)
        {
            var order = await unitOfWork.OrderRepository.FindById(orderId);
            if (order == null || !OrderStatuses.CanTransition(order.Status, OrderStatuses.Cancelled))
                continue;

            order.Status = OrderStatuses.Cancelled;
            var payment = await unitOfWork.PaymentRepository.Find(x => x.OrderId == orderId)
                .FirstOrDefaultAsync(cancellationToken);
            payment?.MarkAsCancelled();
            await inventoryService.ReleaseAsync(orderId, "reservation-expired", cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
