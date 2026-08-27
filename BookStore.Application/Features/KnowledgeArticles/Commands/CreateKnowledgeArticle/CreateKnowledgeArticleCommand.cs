using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.CreateKnowledgeArticle
{
    public class CreateKnowledgeArticleCommand : IRequest<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
    }
}
