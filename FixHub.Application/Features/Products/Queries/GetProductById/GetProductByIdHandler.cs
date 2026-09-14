using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Products.DTOs;
using MediatR;
using FixHub.Application.Common.Exceptions;
using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
            var product = await _unitOfWork.ProductRepository.GetAll()
                .Include(x => x.Category)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == request.Id && (request.IncludeInactive || x.IsActive), cancellationToken);
            if (product == null) throw new NotFoundException(nameof(Product), request.Id);
            return _mapper.Map<ProductResponse>(product);
        }
    }
}
