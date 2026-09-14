using FixHub.Application.Features.KnowledgeArticles.Commands.CreateKnowledgeArticle;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.BulkCreateKnowledgeArticles;

public class BulkCreateKnowledgeArticlesCommand : IRequest<List<Guid>>
{
    public List<CreateKnowledgeArticleCommand> Articles { get; set; } = [];
}
