using BookStore.Application.Common.Models;
using BookStore.Application.Features.Auth.Commands.Login;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);

            Response.Cookies.Append(
            "refreshToken",
            response.RefeshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            response.RefeshToken = string.Empty;


            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successfully",
                Data = response
            });
        }

        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> Refresh()
        //{
        //    var refreshToken = Request.Cookies["refreshToken"];

        //    var response = await _mediator.Send(
        //        new RefreshTokenCommand(refreshToken));

        //    Response.Cookies.Append(
        //        "refreshToken",
        //        response.RefreshToken,
        //        CookieOptions());

        //    response.RefreshToken = "";

        //    return Ok(response);
        //}

    }

    
}
