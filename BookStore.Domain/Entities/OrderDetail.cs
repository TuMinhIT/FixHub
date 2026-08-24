namespace FixHub.Domain.Entities
{
    public class OrderDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        public Guid? RepairServiceId { get; set; }
        public RepairService? RepairService { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }
}
