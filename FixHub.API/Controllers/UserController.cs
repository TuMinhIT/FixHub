using FixHub.Application.Common.Models;
using FixHub.Application.Features.Users.Commands.UpdateInfo;
using FixHub.Application.Features.Users.Queries.GetAllUsers;
using FixHub.Application.Features.Users.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController(IMediator _mediator) : ControllerBase
    {
        [HttpGet("all")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> getAllUser(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllUserQuery(), cancellationToken);
            return Ok(new ApiResponse<List<UserResponse>>(response));
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> updateUserProfile([FromBody] UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<UpdateProfileResponse>(response, "Profile updated successfully"));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> getUserProfile(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetProfileQuery(), cancellationToken);
            return Ok(new ApiResponse<GetProfileResponse>(response));
        }
    }
}
