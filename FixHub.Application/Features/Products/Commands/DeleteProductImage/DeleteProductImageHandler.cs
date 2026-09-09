using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Products.Commands.DeleteProductImage;

public sealed class DeleteProductImageHandler : IRequestHandler<DeleteProductImageCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductImageHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _unitOfWork.ProductImageRepository.Find(x => x.Id == request.ImageId && x.ProductId == request.ProductId)
            .FirstOrDefaultAsync(cancellationToken);
        if (image == null)
            throw new NotFoundException(nameof(ProductImage), request.ImageId);

        await _unitOfWork.ProductImageRepository.DeleteAsync(image.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
