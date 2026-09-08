

namespace FixHub.Application.Features.Users.Queries.GetAllUsers
{
    public class UserResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? Dob { get; set; }

        public string? Gender { get; set; }
        public bool? IsActive { get; set; }
        public string? Avatar { get; set; } = string.Empty;


    }
}
