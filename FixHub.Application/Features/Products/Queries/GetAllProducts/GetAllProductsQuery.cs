using FixHub.Application.Common.Models;
using FixHub.Application.Features.Products.DTOs;
using MediatR;

namespace FixHub.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<Pagination<ProductResponse>>
    {
        public string? Q { get; set; }
        public Guid? CategoryId { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? CapacityHp { get; set; }
        public bool? Inverter { get; set; }
        public bool? IsActive { get; set; }
        public string? Sort { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
