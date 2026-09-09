using MediatR;

namespace FixHub.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>
    {
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
        public bool IsActive { get; set; } = true;
        public Guid CategoryId { get; set; }
    }
}
