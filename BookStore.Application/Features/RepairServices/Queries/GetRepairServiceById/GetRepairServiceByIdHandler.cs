using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.RepairServices.DTOs;
using MediatR;

namespace FixHub.Application.Features.RepairServices.Queries.GetRepairServiceById
{
    public class GetRepairServiceByIdHandler : IRequestHandler<GetRepairServiceByIdQuery, RepairServiceResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRepairServiceByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RepairServiceResponse> Handle(GetRepairServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository.FindById(request.Id);
            if (service == null) throw new Exception("Service not found");
            return _mapper.Map<RepairServiceResponse>(service);
        }
    }
}
