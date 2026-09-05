namespace FixHub.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        
        // e.g., Pending, Confirmed, InProgress, Completed, Cancelled
        public string Status { get; set; } = "Pending"; 
        
        // Contact info
        public string? Note { get; set; }
        public Guid? AddressId { get; set; }
        public Address? Address { get; set; }
        public string? IdempotencyKey { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public Payment? Payment { get; set; }
    }
}
