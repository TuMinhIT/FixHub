using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.FindById(request.Id);
            if (category == null) throw new Exception("Category not found");

            await _unitOfWork.CategoryRepository.DeleteAsync(category.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
