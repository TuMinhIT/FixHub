using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Interfaces.Rag;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.CreateKnowledgeArticle
{
    public class CreateKnowledgeArticleHandler : IRequestHandler<CreateKnowledgeArticleCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRagIndexQueue _ragIndexQueue;

        public CreateKnowledgeArticleHandler(IUnitOfWork unitOfWork, IMapper mapper, IRagIndexQueue ragIndexQueue)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ragIndexQueue = ragIndexQueue;
        }

        public async Task<Guid> Handle(CreateKnowledgeArticleCommand request, CancellationToken cancellationToken)
        {
            var article = _mapper.Map<KnowledgeArticle>(request);
            
            await _unitOfWork.KnowledgeArticleRepository.AddAsync(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _ragIndexQueue.EnqueueAsync(article.Id, cancellationToken);
            return article.Id;
        }
    }
}
