using MediatR;


namespace BookStore.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<RegisterResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get;  set; } = string.Empty;
    }
}
