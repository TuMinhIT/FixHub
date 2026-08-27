using FixHub.Application.Features.KnowledgeArticles.DTOs;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Queries.GetKnowledgeArticleById
{
    public class GetKnowledgeArticleByIdQuery : IRequest<KnowledgeArticleResponse>
    {
        public Guid Id { get; set; }
        public GetKnowledgeArticleByIdQuery(Guid id) => Id = id;
    }
}
