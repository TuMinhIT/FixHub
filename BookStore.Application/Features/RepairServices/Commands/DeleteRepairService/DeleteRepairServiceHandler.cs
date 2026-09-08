using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.RepairServices.Commands.DeleteRepairService
{
    public class DeleteRepairServiceHandler : IRequestHandler<DeleteRepairServiceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRepairServiceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteRepairServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository.FindById(request.Id);
            if (service == null) throw new FixHub.Application.Common.Exceptions.NotFoundException(nameof(FixHub.Domain.Entities.RepairService), request.Id);

            await _unitOfWork.ServiceRepository.DeleteAsync(service.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
