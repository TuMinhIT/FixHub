namespace BookStore.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;

        public DateTime CreateAt { get; set; }
        = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
