using FixHub.Application.Common.Models;
using FixHub.Application.Features.RepairServices.Commands.CreateRepairService;
using FixHub.Application.Features.RepairServices.Commands.DeleteRepairService;
using FixHub.Application.Features.RepairServices.Commands.UpdateRepairService;
using FixHub.Application.Features.RepairServices.DTOs;
using FixHub.Application.Features.RepairServices.Queries.GetAllRepairServices;
using FixHub.Application.Features.RepairServices.Queries.GetRepairServiceById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/repairservice")]
    [Route("api/v1/repair-services")]
    public class RepairServiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RepairServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRepairServices(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllRepairServicesQuery(), cancellationToken);
            return Ok(new ApiResponse<List<RepairServiceResponse>>(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRepairServiceById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetRepairServiceByIdQuery(id), cancellationToken);
            return Ok(new ApiResponse<RepairServiceResponse>(response));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRepairService([FromBody] CreateRepairServiceCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<Guid>(response, "Service created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRepairService(Guid id, [FromBody] UpdateRepairServiceCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Service updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRepairService(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteRepairServiceCommand(id), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Service deleted successfully"));
        }
    }
}
