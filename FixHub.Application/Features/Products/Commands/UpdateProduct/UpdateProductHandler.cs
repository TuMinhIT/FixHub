using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FixHub.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.FindById(request.Id);
            if (product == null) throw new NotFoundException(nameof(Product), request.Id);
            if (request.Price < 0 || request.StockQuantity < 0)
                throw new BadRequestException("Price and stock quantity cannot be negative.");
            ValidateSpecifications(request.SpecificationsJson);
            var categoryExists = await _unitOfWork.CategoryRepository.GetAll()
                .AnyAsync(x => x.Id == request.CategoryId && x.IsActive, cancellationToken);
            if (!categoryExists)
                throw new NotFoundException(nameof(Category), request.CategoryId);
            
            _mapper.Map(request, product);
            product.Sku = string.IsNullOrWhiteSpace(request.Sku) ? product.Sku : request.Sku.Trim().ToUpperInvariant();
            product.Brand = request.Brand?.Trim();
            product.Model = request.Model?.Trim();
            product.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.ProductRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
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
