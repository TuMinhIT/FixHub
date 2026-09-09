namespace FixHub.Domain.Entities;

public enum InventoryTransactionType
{
    Reserved = 1,
    Released = 2,
    Sold = 3,
    Adjustment = 4
}

public class InventoryTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }
    public InventoryTransactionType Type { get; set; }
    public int Quantity { get; set; }
    public string? Reference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
