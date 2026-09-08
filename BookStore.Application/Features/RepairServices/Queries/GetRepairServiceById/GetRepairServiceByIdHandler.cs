using AutoMapper;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Features.RepairServices.DTOs;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var service = await _unitOfWork.ServiceRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken);
            if (service == null) throw new NotFoundException(nameof(RepairService), request.Id);
            return _mapper.Map<RepairServiceResponse>(service);
        }
    }
}
