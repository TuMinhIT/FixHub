using BookStore.Application.Common.Models;
using BookStore.Application.Features.Users.Queries.GetAllUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(IMediator _mediator): ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> getAllUser(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllUserQuery(), cancellationToken);
            return Ok(new ApiResponse<List<UserResponse>>(response));
        }
    }
}
