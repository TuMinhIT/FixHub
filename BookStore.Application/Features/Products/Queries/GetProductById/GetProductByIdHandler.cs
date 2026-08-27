using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Products.DTOs;
using MediatR;

namespace FixHub.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.FindById(request.Id);
            if (product == null) throw new Exception("Product not found");
            return _mapper.Map<ProductResponse>(product);
        }
    }
}
