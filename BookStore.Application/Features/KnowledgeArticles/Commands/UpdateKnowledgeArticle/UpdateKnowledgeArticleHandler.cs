using AutoMapper;
using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.UpdateKnowledgeArticle
{
    public class UpdateKnowledgeArticleHandler : IRequestHandler<UpdateKnowledgeArticleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateKnowledgeArticleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateKnowledgeArticleCommand request, CancellationToken cancellationToken)
        {
            var article = await _unitOfWork.KnowledgeArticleRepository.FindById(request.Id);
            if (article == null) throw new Exception("Article not found");
            
            _mapper.Map(request, article);

            await _unitOfWork.KnowledgeArticleRepository.UpdateAsync(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
