using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.DeleteKnowledgeArticle
{
    public class DeleteKnowledgeArticleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public DeleteKnowledgeArticleCommand(Guid id) => Id = id;
    }
}
