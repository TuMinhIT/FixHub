using FixHub.Application.Features.Carts.Services;
using FixHub.Domain.Entities;
using Xunit;

namespace FixHub.Tests;

public class CartResponseFactoryTests
{
    [Fact]
    public void Calculates_cart_subtotal_from_current_product_prices()
    {
        var product = new Product
        {
            Name = "Máy lạnh 1HP",
            Sku = "AC-1HP",
            Price = 8_500_000,
            StockQuantity = 5
        };
        var cart = new Cart();
        cart.Items.Add(new CartItem
        {
            Cart = cart,
            Product = product,
            ProductId = product.Id,
            Quantity = 2
        });

        var response = CartResponseFactory.Create(cart);

        Assert.Equal(17_000_000, response.Subtotal);
        Assert.Single(response.Items);
        Assert.Equal(5, response.Items[0].AvailableStock);
    }
}
