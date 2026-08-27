using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.UpdateKnowledgeArticle
{
    public class UpdateKnowledgeArticleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Tags { get; set; }
    }
}
