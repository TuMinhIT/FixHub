using MediatR;


namespace FixHub.Application.Features.Auth.Commands.Login
{
    public class LoginCommand:IRequest<LoginResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } 
    }
}
