using AutoMapper;
using FixHub.Application.Features.Products.Commands.CreateProduct;
using FixHub.Application.Features.Products.Commands.UpdateProduct;
using FixHub.Application.Features.Products.DTOs;
using FixHub.Domain.Entities;

namespace FixHub.Application.Common.Mappings
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductResponse>();
            CreateMap<CreateProductCommand, Product>();
            CreateMap<UpdateProductCommand, Product>();
        }
    }
}
