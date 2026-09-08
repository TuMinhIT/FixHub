namespace FixHub.Application.Common.Interfaces;

public interface IRagIndexQueue
{
    ValueTask EnqueueAsync(Guid articleId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken = default);
}
