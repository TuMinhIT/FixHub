using FixHub.Application.Features.KnowledgeArticles.DTOs;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Queries.GetAllKnowledgeArticles
{
    public class GetAllKnowledgeArticlesQuery : IRequest<List<KnowledgeArticleResponse>>
    {
    }
}
