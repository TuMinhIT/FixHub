using FixHub.Application.Features.RepairServices.DTOs;
using MediatR;

namespace FixHub.Application.Features.RepairServices.Queries.GetRepairServiceById
{
    public class GetRepairServiceByIdQuery : IRequest<RepairServiceResponse>
    {
        public Guid Id { get; set; }
        public GetRepairServiceByIdQuery(Guid id) => Id = id;
    }
}
