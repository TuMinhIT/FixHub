using MediatR;

namespace FixHub.Application.Features.Users.Commands.UpdateInfo
{
    public class UpdateProfileCommand : IRequest<UpdateProfileResponse>
    {
        public string? Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? Avatar { get; set; }
    }
}

