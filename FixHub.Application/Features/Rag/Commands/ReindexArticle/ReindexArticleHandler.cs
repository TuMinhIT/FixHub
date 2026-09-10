using FixHub.Application.Common.Interfaces.Rag;
using MediatR;

namespace FixHub.Application.Features.Rag.Commands.ReindexArticle;

public sealed class ReindexArticleHandler : IRequestHandler<ReindexArticleCommand, bool>
{
    private readonly IRagIndexingService _indexingService;

    public ReindexArticleHandler(IRagIndexingService indexingService)
    {
        _indexingService = indexingService;
    }

    public async Task<bool> Handle(ReindexArticleCommand request, CancellationToken cancellationToken)
    {
        await _indexingService.IndexArticleAsync(request.ArticleId, cancellationToken);
        return true;
    }
}
