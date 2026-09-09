using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.FindById(request.Id);
            if (product == null) throw new FixHub.Application.Common.Exceptions.NotFoundException(nameof(FixHub.Domain.Entities.Product), request.Id);

            await _unitOfWork.ProductRepository.DeleteAsync(product.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
