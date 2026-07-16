namespace BookStore.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        public bool IsActive { get; set; }

        public DateTime CreateAt { get; set; }
        = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        //public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
    }
}
