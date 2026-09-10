using FixHub.Application.Common.Models;
using FixHub.Application.Features.Auth.Commands.Login;
using FixHub.Application.Features.Auth.Commands.logout;
using FixHub.Application.Features.Auth.Commands.RefreshToken;
using FixHub.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
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
                SameSite = SameSiteMode.None,
                Path = "/",
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Refresh token not found.",
                    Data = null
                });
            }

            var response = await _mediator.Send(
                new RefreshTokenCommand(refreshToken));

            Response.Cookies.Append(
                "refreshToken",
                response.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });


            response.RefreshToken = "";

            return Ok(new ApiResponse<RefreshTokenResponse>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = response
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Refresh token not found.",
                    Data = null
                });
            }

            var result = await _mediator.Send(
                new LogoutCommand(refreshToken),
                cancellationToken);

            if (!result)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid refresh token.",
                    Data = null
                });
            }

            //Response.Cookies.Delete("refreshToken");

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });


            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Logout successfully.",
                Data = null
            });
        }
    }

    
}
