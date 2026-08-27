using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.KnowledgeArticles.DTOs;
using MediatR;

namespace FixHub.Application.Features.KnowledgeArticles.Queries.GetKnowledgeArticleById
{
    public class GetKnowledgeArticleByIdHandler : IRequestHandler<GetKnowledgeArticleByIdQuery, KnowledgeArticleResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetKnowledgeArticleByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<KnowledgeArticleResponse> Handle(GetKnowledgeArticleByIdQuery request, CancellationToken cancellationToken)
        {
            var article = await _unitOfWork.KnowledgeArticleRepository.FindById(request.Id);
            if (article == null) throw new Exception("Article not found");
            return _mapper.Map<KnowledgeArticleResponse>(article);
        }
    }
}
