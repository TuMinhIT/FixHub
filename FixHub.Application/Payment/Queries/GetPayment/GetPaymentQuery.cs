using MediatR;

namespace FixHub.Application.Payment.Queries.GetPayment;

public sealed record GetPaymentQuery(Guid PaymentId) : IRequest<PaymentResponse>;
