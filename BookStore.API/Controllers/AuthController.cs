using BookStore.Application.Common.Models;
using BookStore.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [ApiController]

    [Route("api/auth")]
    public class AuthController(IMediator _mediator) : ControllerBase
    {   
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponse<RegisterResponse>
            {
                Success = true,
                Message = "Register successfully",
                Data = response
            });
        }
    }
}
