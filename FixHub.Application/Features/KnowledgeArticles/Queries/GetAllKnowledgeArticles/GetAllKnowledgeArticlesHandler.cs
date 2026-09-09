using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.KnowledgeArticles.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.KnowledgeArticles.Queries.GetAllKnowledgeArticles
{
    public class GetAllKnowledgeArticlesHandler : IRequestHandler<GetAllKnowledgeArticlesQuery, List<KnowledgeArticleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllKnowledgeArticlesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<KnowledgeArticleResponse>> Handle(GetAllKnowledgeArticlesQuery request, CancellationToken cancellationToken)
        {
            var articles = await _unitOfWork.KnowledgeArticleRepository.GetAll()
                .Where(x => x.Status == Domain.Entities.KnowledgeArticleStatuses.Published)
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<KnowledgeArticleResponse>>(articles);
        }
    }
}
