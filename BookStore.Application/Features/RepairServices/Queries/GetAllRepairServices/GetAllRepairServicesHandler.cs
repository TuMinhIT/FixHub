using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.RepairServices.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.RepairServices.Queries.GetAllRepairServices
{
    public class GetAllRepairServicesHandler : IRequestHandler<GetAllRepairServicesQuery, List<RepairServiceResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllRepairServicesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<RepairServiceResponse>> Handle(GetAllRepairServicesQuery request, CancellationToken cancellationToken)
        {
            var services = await _unitOfWork.ServiceRepository.GetAll().ToListAsync(cancellationToken);
            return _mapper.Map<List<RepairServiceResponse>>(services);
        }
    }
}
