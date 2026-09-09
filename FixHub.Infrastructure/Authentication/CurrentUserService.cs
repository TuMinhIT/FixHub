using FixHub.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace FixHub.Infrastructure.Authentication
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                if (!Guid.TryParse(value, out var userId))
                    throw new UnauthorizedAccessException("Authenticated user id is missing or invalid.");

                return userId;
            }
        }

        public Guid? UserIdOrNull => IsAuthenticated && Guid.TryParse(
            _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out var userId) ? userId : null;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

        public string Email =>
            _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value
            ?? throw new UnauthorizedAccessException("Authenticated email is missing.");

        public string Role =>
            _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value
            ?? throw new UnauthorizedAccessException("Authenticated role is missing.");
    }
}
