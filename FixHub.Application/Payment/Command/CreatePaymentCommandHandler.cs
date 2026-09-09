using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Payment.Command
{
    public sealed class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ICurrentUserService _currentUserService;

        public CreatePaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _currentUserService = currentUserService;
        }

        public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository
                .GetAll()
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                throw new NotFoundException(nameof(Order), request.OrderId);
            }

            if (order.UserId != _currentUserService.UserId
                && !string.Equals(_currentUserService.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                throw new ForbiddenException("You do not have access to this order.");

            if (order.Status == OrderStatuses.Paid)
            {
                throw new BadRequestException("Order is already paid.");
            }
            if (order.Status != OrderStatuses.PendingPayment)
                throw new BadRequestException($"Payment cannot be created for order status {order.Status}.");

            var idempotencyKey = string.IsNullOrWhiteSpace(request.IdempotencyKey)
                ? $"payment:{request.OrderId}"
                : request.IdempotencyKey.Trim();

            var existingPayment = await _unitOfWork.PaymentRepository
                .GetAll()
                .FirstOrDefaultAsync(p => p.OrderId == order.Id, cancellationToken);

            if (existingPayment != null)
            {
                return new CreatePaymentResponse(
                    existingPayment.Id,
                    existingPayment.CheckoutUrl ?? string.Empty,
                    existingPayment.GetCheckoutFields());
                }

            var keyAlreadyUsed = await _unitOfWork.PaymentRepository.GetAll()
                .AnyAsync(p => p.IdempotencyKey == idempotencyKey && p.OrderId != order.Id, cancellationToken);
            if (keyAlreadyUsed)
                throw new BadRequestException("Idempotency key has already been used for another payment.");

            var invoiceNumber = $"ORDER_{order.Id:N}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            var payment = FixHub.Domain.Entities.Payment.Create(order.Id, order.TotalAmount, invoiceNumber, idempotencyKey);

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdCheckout = await _paymentGateway.CreateCheckoutAsync(
                new PaymentCheckoutRequest(
                    invoiceNumber,
                    payment.Amount,
                    $"Thanh toán đơn hàng {order.Id}",
                    _paymentGateway.CheckoutUrls.SuccessUrl,
                    _paymentGateway.CheckoutUrls.ErrorUrl,
                    _paymentGateway.CheckoutUrls.CancelUrl),
                cancellationToken);

            payment.SetCheckoutData(createdCheckout.CheckoutUrl, createdCheckout.Fields);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreatePaymentResponse(
                payment.Id,
                payment.CheckoutUrl ?? string.Empty,
                payment.GetCheckoutFields());
        }
    }
}
