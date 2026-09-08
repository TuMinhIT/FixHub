using FixHub.Domain.Entities;
using Xunit;

namespace FixHub.Tests;

public class OrderStatusesTests
{
    [Theory]
    [InlineData(OrderStatuses.PendingPayment, OrderStatuses.Paid)]
    [InlineData(OrderStatuses.PendingPayment, OrderStatuses.Cancelled)]
    [InlineData(OrderStatuses.Paid, OrderStatuses.Processing)]
    [InlineData(OrderStatuses.Processing, OrderStatuses.Shipping)]
    [InlineData(OrderStatuses.Shipping, OrderStatuses.Completed)]
    public void Allows_valid_transitions(string current, string next)
    {
        Assert.True(OrderStatuses.CanTransition(current, next));
    }

    [Theory]
    [InlineData(OrderStatuses.PendingPayment, OrderStatuses.Completed)]
    [InlineData(OrderStatuses.Completed, OrderStatuses.Cancelled)]
    [InlineData(OrderStatuses.Cancelled, OrderStatuses.Paid)]
    public void Rejects_invalid_transitions(string current, string next)
    {
        Assert.False(OrderStatuses.CanTransition(current, next));
    }
}
