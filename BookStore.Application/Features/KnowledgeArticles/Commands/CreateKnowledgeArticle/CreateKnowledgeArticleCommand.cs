using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.CreateKnowledgeArticle
{
    public class CreateKnowledgeArticleCommand : IRequest<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public string Status { get; set; } = Domain.Entities.KnowledgeArticleStatuses.Published;
        public string? Source { get; set; }
    }
}
