using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.CreateKnowledgeArticle
{
    public class CreateKnowledgeArticleHandler : IRequestHandler<CreateKnowledgeArticleCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateKnowledgeArticleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateKnowledgeArticleCommand request, CancellationToken cancellationToken)
        {
            var article = _mapper.Map<KnowledgeArticle>(request);
            
            await _unitOfWork.KnowledgeArticleRepository.AddAsync(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return article.Id;
        }
    }
}
