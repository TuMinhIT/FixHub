using AutoMapper;
using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.RepairServices.Commands.UpdateRepairService
{
    public class UpdateRepairServiceHandler : IRequestHandler<UpdateRepairServiceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateRepairServiceHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateRepairServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository.FindById(request.Id);
            if (service == null) throw new FixHub.Application.Common.Exceptions.NotFoundException(nameof(FixHub.Domain.Entities.RepairService), request.Id);
            
            _mapper.Map(request, service);
            await _unitOfWork.ServiceRepository.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
