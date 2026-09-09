using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Payment.Queries.GetPayment;

public sealed class GetPaymentHandler : IRequestHandler<GetPaymentQuery, PaymentResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetPaymentHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<PaymentResponse> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.PaymentRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == request.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        var order = await _unitOfWork.OrderRepository.FindById(payment.OrderId);
        if (order == null)
            throw new NotFoundException(nameof(Order), payment.OrderId);
        if (order.UserId != _currentUserService.UserId
            && !string.Equals(_currentUserService.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("You do not have access to this payment.");

        return new PaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Method = payment.Method.ToString(),
            Status = payment.Status.ToString(),
            InvoiceNumber = payment.InvoiceNumber,
            GatewayTransactionId = payment.GatewayTransactionId,
            CheckoutUrl = payment.CheckoutUrl,
            CreatedAt = payment.CreatedAt,
            PaidAt = payment.PaidAt
        };
    }
}
