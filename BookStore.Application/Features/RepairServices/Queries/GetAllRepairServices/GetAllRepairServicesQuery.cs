using FixHub.Application.Features.RepairServices.DTOs;
using MediatR;

namespace FixHub.Application.Features.RepairServices.Queries.GetAllRepairServices
{
    public class GetAllRepairServicesQuery : IRequest<List<RepairServiceResponse>>
    {
    }
}
