using System;


namespace FixHub.Application.Features.Auth.Commands.Login
{
    public class UserLoginResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
