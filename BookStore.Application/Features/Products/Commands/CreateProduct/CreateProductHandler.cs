using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FixHub.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (request.Price < 0 || request.StockQuantity < 0)
                throw new BadRequestException("Price and stock quantity cannot be negative.");
            ValidateSpecifications(request.SpecificationsJson);

            var categoryExists = await _unitOfWork.CategoryRepository.GetAll()
                .AnyAsync(x => x.Id == request.CategoryId && x.IsActive, cancellationToken);
            if (!categoryExists)
                throw new NotFoundException(nameof(Category), request.CategoryId);

            var product = _mapper.Map<Product>(request);
            product.Sku = string.IsNullOrWhiteSpace(request.Sku)
                ? $"FIX-{Guid.NewGuid():N}"[..16].ToUpperInvariant()
                : request.Sku.Trim().ToUpperInvariant();
            product.Brand = request.Brand?.Trim();
            product.Model = request.Model?.Trim();
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

        private static void ValidateSpecifications(string? specifications)
        {
            if (string.IsNullOrWhiteSpace(specifications))
                return;

            try
            {
                using var _ = JsonDocument.Parse(specifications);
            }
            catch (JsonException)
            {
                throw new BadRequestException("SpecificationsJson must be valid JSON.");
            }
        }
    }
}
