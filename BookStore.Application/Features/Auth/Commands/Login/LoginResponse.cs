

namespace FixHub.Application.Features.Auth.Commands.Login
{
    public class LoginResponse
    {
        public string RefeshToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserLoginResponse User { get; set; }

    }
}
