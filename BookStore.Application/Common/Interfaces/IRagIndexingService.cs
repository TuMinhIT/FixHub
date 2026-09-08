namespace FixHub.Application.Common.Interfaces;

public interface IRagIndexingService
{
    Task IndexArticleAsync(Guid articleId, CancellationToken cancellationToken = default);
    Task ReindexAllAsync(CancellationToken cancellationToken = default);
}
