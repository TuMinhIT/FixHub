using AutoMapper;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Payment;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaymentGateway _paymentGateway;

        public CreateOrderHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _paymentGateway = paymentGateway;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var idempotencyKey = string.IsNullOrWhiteSpace(request.IdempotencyKey)
                ? Guid.NewGuid().ToString("N")
                : request.IdempotencyKey.Trim();

            if (request.Items == null || !request.Items.Any())
            {
                throw new BadRequestException("Order must contain at least one item.");
            }

            var existingOrder = await _unitOfWork.OrderRepository
                .GetAll()
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.IdempotencyKey == idempotencyKey, cancellationToken);

            if (existingOrder != null)
            {
                if (existingOrder.Payment != null)
                {
                    return new CreateOrderResponse
                    {
                        OrderId = existingOrder.Id,
                        PaymentId = existingOrder.Payment.Id,
                        CheckoutUrl = existingOrder.Payment.CheckoutUrl ?? string.Empty,
                        Fields = existingOrder.Payment.GetCheckoutFields()
                    };
                }

                return new CreateOrderResponse
                {
                    OrderId = existingOrder.Id,
                    Fields = new Dictionary<string, string>()
                };
            }

            var order = new Order
            {
                UserId = userId,
                AddressId = request.AddressId,
                Note = request.Note,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                TotalAmount = request.TotalAmount > 0
                    ? request.TotalAmount
                    : request.Items.Sum(i => i.Quantity * i.UnitPrice),
                IdempotencyKey = idempotencyKey
            };

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new BadRequestException("Quantity must be greater than zero.");
                }

                if (item.ProductId == null && item.RepairServiceId == null)
                {
                    throw new BadRequestException("Each order item must reference a product or repair service.");
                }

                var orderDetail = new OrderDetail
                {
                    Order = order,
                    ProductId = item.ProductId,
                    RepairServiceId = item.RepairServiceId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };

                order.OrderDetails.Add(orderDetail);
            }

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.PaymentMethod.Equals("sepay", StringComparison.OrdinalIgnoreCase))
            {
                var invoiceNumber = $"ORDER_{order.Id:N}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                var payment = FixHub.Domain.Entities.Payment.Create(order.Id, order.TotalAmount, invoiceNumber, idempotencyKey);

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var checkout = await _paymentGateway.CreateCheckoutAsync(
                    new PaymentCheckoutRequest(
                        invoiceNumber,
                        payment.Amount,
                        $"Thanh toán đơn hàng {order.Id}",
                        "http://localhost:5173/orders",
                        "http://localhost:5173/orders/error",
                        "http://localhost:5173/orders/cancel"),
                    cancellationToken);

                payment.SetCheckoutData(checkout.CheckoutUrl, checkout.Fields);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new CreateOrderResponse
                {
                    OrderId = order.Id,
                    PaymentId = payment.Id,
                    CheckoutUrl = payment.CheckoutUrl ?? string.Empty,
                    Fields = payment.GetCheckoutFields()
                };
            }

            return new CreateOrderResponse
            {
                OrderId = order.Id,
                Fields = new Dictionary<string, string>()
            };
        }
    }
}
