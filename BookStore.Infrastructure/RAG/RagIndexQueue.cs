using System.Threading.Channels;
using FixHub.Application.Common.Interfaces;

namespace FixHub.Infrastructure.RAG;

public sealed class RagIndexQueue : IRagIndexQueue
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

    public ValueTask EnqueueAsync(Guid articleId, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(articleId, cancellationToken);

    public IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken = default) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
