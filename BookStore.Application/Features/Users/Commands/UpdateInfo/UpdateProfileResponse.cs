namespace FixHub.Application.Features.Users.Commands.UpdateInfo
{
    public class UpdateProfileResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;
    } 
}
