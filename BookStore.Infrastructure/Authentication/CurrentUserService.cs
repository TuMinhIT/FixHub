using BookStore.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace BookStore.Infrastructure.Authentication
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

                return Guid.Parse(value!);
            }
        }

        public string Email =>
            _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.Email)?
                .Value!;

        public string Role =>
            _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.Role)?
                .Value!;
    }
}
