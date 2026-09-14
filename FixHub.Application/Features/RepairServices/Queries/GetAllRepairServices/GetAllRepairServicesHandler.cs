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
            IQueryable<FixHub.Domain.Entities.RepairService> query = _unitOfWork.ServiceRepository.GetAll();
            if (!request.IncludeInactive)
                query = query.Where(x => x.IsActive);

            var services = await query
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<RepairServiceResponse>>(services);
        }
    }
}
