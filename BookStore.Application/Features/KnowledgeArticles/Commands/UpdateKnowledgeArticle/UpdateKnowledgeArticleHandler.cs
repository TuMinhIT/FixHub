using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.UpdateKnowledgeArticle
{
    public class UpdateKnowledgeArticleHandler : IRequestHandler<UpdateKnowledgeArticleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRagIndexQueue _ragIndexQueue;

        public UpdateKnowledgeArticleHandler(IUnitOfWork unitOfWork, IMapper mapper, IRagIndexQueue ragIndexQueue)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ragIndexQueue = ragIndexQueue;
        }

        public async Task<bool> Handle(UpdateKnowledgeArticleCommand request, CancellationToken cancellationToken)
        {
            var article = await _unitOfWork.KnowledgeArticleRepository.FindById(request.Id);
            if (article == null) throw new NotFoundException(nameof(Domain.Entities.KnowledgeArticle), request.Id);
            
            _mapper.Map(request, article);
            article.Version++;
            article.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.KnowledgeArticleRepository.UpdateAsync(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _ragIndexQueue.EnqueueAsync(article.Id, cancellationToken);
            return true;
        }
    }
}
