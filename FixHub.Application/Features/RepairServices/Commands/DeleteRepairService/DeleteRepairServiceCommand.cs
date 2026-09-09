using MediatR;

namespace FixHub.Application.Features.RepairServices.Commands.DeleteRepairService
{
    public class DeleteRepairServiceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public DeleteRepairServiceCommand(Guid id) => Id = id;
    }
}
