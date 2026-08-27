using MediatR;

namespace FixHub.Application.Features.RepairServices.Commands.UpdateRepairService
{
    public class UpdateRepairServiceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
    }
}
