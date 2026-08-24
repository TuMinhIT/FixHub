
using System.Collections.ObjectModel;

namespace FixHub.Domain.Entities
{
    public class Orders
    {
         public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Shipping, Completed, Cancelled
        public string PaymentMethod { get; set; } // COD, Banking, VNPay
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
