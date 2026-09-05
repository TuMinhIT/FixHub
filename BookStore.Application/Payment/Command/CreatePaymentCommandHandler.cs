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

        public CreatePaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
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

            if (order.Status == "Paid")
            {
                throw new BadRequestException("Order is already paid.");
            }

            var idempotencyKey = string.IsNullOrWhiteSpace(request.IdempotencyKey)
                ? $"payment:{request.OrderId}"
                : request.IdempotencyKey.Trim();

            var existingPayment = await _unitOfWork.PaymentRepository
                .GetAll()
                .FirstOrDefaultAsync(
                    p => p.OrderId == order.Id || p.IdempotencyKey == idempotencyKey,
                    cancellationToken);

            if (existingPayment != null)
            {
                return new CreatePaymentResponse(
                    existingPayment.Id,
                    existingPayment.CheckoutUrl ?? string.Empty,
                    existingPayment.GetCheckoutFields());
            }

            var invoiceNumber = $"ORDER_{order.Id:N}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            var payment = FixHub.Domain.Entities.Payment.Create(order.Id, order.TotalAmount, invoiceNumber, idempotencyKey);

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdCheckout = await _paymentGateway.CreateCheckoutAsync(
                new PaymentCheckoutRequest(
                    invoiceNumber,
                    payment.Amount,
                    $"Thanh toán đơn hàng {order.Id}",
                    "https://localhost:5001/payment/success",
                    "https://localhost:5001/payment/error",
                    "https://localhost:5001/payment/cancel"),
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
