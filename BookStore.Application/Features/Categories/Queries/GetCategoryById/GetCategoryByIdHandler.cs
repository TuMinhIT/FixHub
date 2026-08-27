using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Categories.DTOs;
using MediatR;

namespace FixHub.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.FindById(request.Id);
            if (category == null) throw new Exception("Category not found");
            return _mapper.Map<CategoryResponse>(category);
        }
    }
}
