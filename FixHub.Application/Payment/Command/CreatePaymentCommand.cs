using MediatR;

namespace FixHub.Application.Payment.Command
{
    public sealed record CreatePaymentCommand(
        Guid OrderId,
        string? IdempotencyKey = null
    ) : IRequest<CreatePaymentResponse>;
}
