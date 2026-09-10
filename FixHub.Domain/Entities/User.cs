namespace FixHub.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }= string.Empty;
        public string Avatar { get; set; }= string.Empty;
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;
        public string Gender { get; set; } = string.Empty;
        public DateTime Dob { get; set; } = DateTime.Now;
        public DateTime CreateAt { get; set; }
        = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
