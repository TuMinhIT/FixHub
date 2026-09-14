using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Interfaces.Rag;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.BulkCreateKnowledgeArticles;

public class BulkCreateKnowledgeArticlesHandler : IRequestHandler<BulkCreateKnowledgeArticlesCommand, List<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRagIndexQueue _ragIndexQueue;

    public BulkCreateKnowledgeArticlesHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IRagIndexQueue ragIndexQueue)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _ragIndexQueue = ragIndexQueue;
    }

    public async Task<List<Guid>> Handle(
        BulkCreateKnowledgeArticlesCommand request,
        CancellationToken cancellationToken)
    {
        var articles = _mapper.Map<List<KnowledgeArticle>>(request.Articles);
        await _unitOfWork.KnowledgeArticleRepository.AddRangeAsync(articles);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var article in articles)
        {
            await _ragIndexQueue.EnqueueAsync(article.Id, cancellationToken);
        }

        return articles.Select(article => article.Id).ToList();
    }
}
