using FixHub.Domain.Entities;
using Xunit;

namespace FixHub.Tests;

public class PaymentTests
{
    [Fact]
    public void MarkAsPaid_is_idempotent()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100_000, "INV-1");

        payment.MarkAsPaid("TX-1");
        var paidAt = payment.PaidAt;
        payment.MarkAsPaid("TX-2");

        Assert.Equal(PaymentStatus.Success, payment.Status);
        Assert.Equal("TX-1", payment.GatewayTransactionId);
        Assert.Equal(paidAt, payment.PaidAt);
    }

    [Fact]
    public void Failed_or_cancelled_payment_cannot_override_success()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100_000, "INV-2");
        payment.MarkAsPaid("TX-1");

        payment.MarkAsFailed();
        payment.MarkAsCancelled();

        Assert.Equal(PaymentStatus.Success, payment.Status);
    }
}
