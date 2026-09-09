namespace FixHub.Domain.Entities;

public static class OrderStatuses
{
    public const string PendingPayment = "PendingPayment";
    public const string Paid = "Paid";
    public const string Processing = "Processing";
    public const string Shipping = "Shipping";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
    public const string Refunded = "Refunded";

    public static bool CanTransition(string current, string next)
    {
        var normalizedCurrent = Normalize(current);
        var normalizedNext = Normalize(next);

        return (normalizedCurrent, normalizedNext) switch
        {
            (PendingPayment, Paid) => true,
            (PendingPayment, Cancelled) => true,
            (Paid, Processing) => true,
            (Paid, Refunded) => true,
            (Processing, Shipping) => true,
            (Processing, Cancelled) => true,
            (Shipping, Completed) => true,
            (Shipping, Cancelled) => true,
            _ when string.Equals(normalizedCurrent, normalizedNext, StringComparison.OrdinalIgnoreCase) => true,
            _ => false
        };
    }

    private static string Normalize(string status) =>
        status.Equals("Pending", StringComparison.OrdinalIgnoreCase)
            ? PendingPayment
            : new[]
            {
                PendingPayment, Paid, Processing, Shipping,
                Completed, Cancelled, Refunded
            }.FirstOrDefault(x => string.Equals(x, status, StringComparison.OrdinalIgnoreCase))
            ?? status;
}
