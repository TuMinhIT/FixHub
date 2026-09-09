namespace FixHub.Application.Features.Carts.DTOs;

public sealed class CartResponse
{
    public Guid Id { get; init; }
    public IReadOnlyList<CartItemResponse> Items { get; init; } = Array.Empty<CartItemResponse>();
    public decimal Subtotal { get; init; }
}

public sealed class CartItemResponse
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public int AvailableStock { get; init; }
    public decimal Subtotal => UnitPrice * Quantity;
    public string? ImageUrl { get; init; }
}
