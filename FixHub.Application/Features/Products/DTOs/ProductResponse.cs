namespace FixHub.Application.Features.Products.DTOs
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SpecificationsJson { get; set; }
        public decimal? CapacityHp { get; set; }
        public int? CapacityBtu { get; set; }
        public bool IsInverter { get; set; }
        public string? EnergyRating { get; set; }
        public string? Refrigerant { get; set; }
        public int? WarrantyMonths { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
        public CategorySummary? Category { get; set; }
        public IReadOnlyList<ProductImageResponse> Images { get; set; } = Array.Empty<ProductImageResponse>();
    }

    public sealed class CategorySummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class ProductImageResponse
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? PublicId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
