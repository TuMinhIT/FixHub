namespace FixHub.Application.Common.Interfaces;

public interface IOrderInventoryService
{
    Task ReleaseAsync(Guid orderId, string reference, CancellationToken cancellationToken = default);
    Task FinalizeAsync(Guid orderId, string reference, CancellationToken cancellationToken = default);
}
