using FixHub.Application.Common.Models;
using FixHub.Application.Features.Addresses.Commands.CreateAddress;
using FixHub.Application.Features.Addresses.Commands.DeleteAddress;
using FixHub.Application.Features.Addresses.Commands.UpdateAddress;
using FixHub.Application.Features.Addresses.DTOs;
using FixHub.Application.Features.Addresses.Queries.GetAddressById;
using FixHub.Application.Features.Addresses.Queries.GetMyAddresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/v1/address")]
    public class AddressController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyAddresses(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetMyAddressesQuery(), cancellationToken);
            return Ok(new ApiResponse<List<AddressResponse>>(response));
        }

        [HttpGet("me/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAddressByIdQuery(id), cancellationToken);
            return Ok(new ApiResponse<AddressResponse>(response));
        }

      
        [HttpPost("me")]
        [Authorize]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<Guid>(response, "Address created successfully"));
        }

       
        [HttpPut("me/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
            {
                return BadRequest("Id mismatch");
            }

            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Address updated successfully"));
        }

        
        [HttpDelete("me/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteAddressCommand(id), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Address deleted successfully"));
        }
    }
}
