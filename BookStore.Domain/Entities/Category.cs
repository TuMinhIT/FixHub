namespace FixHub.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<RepairService> RepairServices { get; set; } = new List<RepairService>();
    }
}
