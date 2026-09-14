using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoriesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<FixHub.Domain.Entities.Category> query = _unitOfWork.CategoryRepository.GetAll();
            if (!request.IncludeInactive)
                query = query.Where(x => x.IsActive);

            var categories = await query
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<CategoryResponse>>(categories);
        }
    }
}
