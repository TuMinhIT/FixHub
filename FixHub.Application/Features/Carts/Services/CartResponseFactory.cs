using FixHub.Application.Features.Carts.DTOs;
using FixHub.Domain.Entities;

namespace FixHub.Application.Features.Carts.Services;

public static class CartResponseFactory
{
    public static CartResponse Create(Cart cart)
    {
        var items = cart.Items.Select(item => new CartItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Sku = item.Product.Sku,
            UnitPrice = item.Product.Price,
            Quantity = item.Quantity,
            AvailableStock = item.Product.StockQuantity,
            ImageUrl = item.Product.Images.FirstOrDefault(x => x.IsPrimary)?.ImageUrl
                ?? item.Product.Images.FirstOrDefault()?.ImageUrl
        }).ToList();

        return new CartResponse
        {
            Id = cart.Id,
            Items = items,
            Subtotal = items.Sum(x => x.Subtotal)
        };
    }
}
