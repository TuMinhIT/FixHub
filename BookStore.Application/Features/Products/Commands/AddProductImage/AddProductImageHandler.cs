using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Products.Commands.AddProductImage;

public sealed class AddProductImageHandler : IRequestHandler<AddProductImageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddProductImageHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
            throw new BadRequestException("Image URL is required.");

        var exists = await _unitOfWork.ProductRepository.GetAll()
            .AnyAsync(x => x.Id == request.ProductId, cancellationToken);
        if (!exists)
            throw new NotFoundException(nameof(Product), request.ProductId);

        var currentImages = await _unitOfWork.ProductImageRepository
            .Find(x => x.ProductId == request.ProductId)
            .ToListAsync(cancellationToken);
        if (request.IsPrimary || currentImages.Count == 0)
            foreach (var image in currentImages)
                image.IsPrimary = false;

        var entity = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            ImageUrl = request.ImageUrl.Trim(),
            PublicId = request.PublicId?.Trim(),
            IsPrimary = request.IsPrimary || currentImages.Count == 0
        };
        await _unitOfWork.ProductImageRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
