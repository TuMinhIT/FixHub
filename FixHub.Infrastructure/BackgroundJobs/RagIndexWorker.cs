using FixHub.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FixHub.Infrastructure.BackgroundJobs;

public sealed class RagIndexWorker : BackgroundService
{
    private readonly IRagIndexQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RagIndexWorker> _logger;

    public RagIndexWorker(
        IRagIndexQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<RagIndexWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var articleId in _queue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var indexingService = scope.ServiceProvider.GetRequiredService<IRagIndexingService>();
                    await indexingService.IndexArticleAsync(articleId, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Failed to index knowledge article {ArticleId}.", articleId);
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }
}
