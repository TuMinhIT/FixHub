namespace FixHub.Application.Common.Interfaces.Rag;

public interface IRagIndexingService
{
    Task IndexArticleAsync(Guid articleId, CancellationToken cancellationToken = default);
    Task ReindexAllAsync(CancellationToken cancellationToken = default);
}
