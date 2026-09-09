using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.RepairServices.Commands.CreateRepairService
{
    public class CreateRepairServiceHandler : IRequestHandler<CreateRepairServiceCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateRepairServiceHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateRepairServiceCommand request, CancellationToken cancellationToken)
        {
            var service = _mapper.Map<RepairService>(request);
            await _unitOfWork.ServiceRepository.AddAsync(service);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return service.Id;
        }
    }
}
