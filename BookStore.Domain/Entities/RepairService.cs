namespace FixHub.Domain.Entities
{
    public class RepairService
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
