using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Features.KnowledgeArticles.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var query = _unitOfWork.KnowledgeArticleRepository.GetAll()
                .Where(x => x.Id == request.Id);
            if (!request.IncludeAllStatuses)
            {
                query = query.Where(x => x.Status == Domain.Entities.KnowledgeArticleStatuses.Published);
            }

            var article = await query.FirstOrDefaultAsync(cancellationToken);
            if (article == null) throw new NotFoundException(nameof(Domain.Entities.KnowledgeArticle), request.Id);
            return _mapper.Map<KnowledgeArticleResponse>(article);
        }
    }
}
