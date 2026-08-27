using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.DeleteKnowledgeArticle
{
    public class DeleteKnowledgeArticleHandler : IRequestHandler<DeleteKnowledgeArticleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteKnowledgeArticleHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteKnowledgeArticleCommand request, CancellationToken cancellationToken)
        {
            var article = await _unitOfWork.KnowledgeArticleRepository.FindById(request.Id);
            if (article == null) throw new Exception("Article not found");

            await _unitOfWork.KnowledgeArticleRepository.DeleteAsync(article.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
