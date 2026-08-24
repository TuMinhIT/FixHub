

namespace FixHub.Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; } // Foreign key to User
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        // Navigation property
        public User User { get; set; }
    }
}
