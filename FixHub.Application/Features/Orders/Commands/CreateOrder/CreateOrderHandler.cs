using AutoMapper;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Payment;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentEntity = FixHub.Domain.Entities.Payment;

namespace FixHub.Application.Features.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IOrderInventoryService _inventoryService;

        public CreateOrderHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IPaymentGateway paymentGateway,
            IOrderInventoryService inventoryService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _paymentGateway = paymentGateway;
            _inventoryService = inventoryService;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var idempotencyKey = string.IsNullOrWhiteSpace(request.IdempotencyKey)
                ? Guid.NewGuid().ToString("N")
                : request.IdempotencyKey.Trim();

            Cart? checkoutCart = null;
            if (request.Items is not { Count: > 0 } && request.CartId.HasValue)
            {
                checkoutCart = await _unitOfWork.CartRepository.GetByUserIdAsync(userId, cancellationToken);
                if (checkoutCart == null || checkoutCart.Id != request.CartId.Value)
                    throw new NotFoundException(nameof(Cart), request.CartId.Value);

                request.Items = checkoutCart.Items
                    .Select(x => new OrderItemRequest { ProductId = x.ProductId, Quantity = x.Quantity })
                    .ToList();
            }

            if (request.Items is not { Count: > 0 })
            {
                throw new BadRequestException("Order must contain at least one item.");
            }
            if (!request.PaymentMethod.Equals("sepay", StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Only SePay payment is supported.");

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

            if (request.AddressId.HasValue)
            {
                var ownsAddress = await _unitOfWork.AddressRepository.GetAll()
                    .AnyAsync(x => x.Id == request.AddressId.Value && x.UserId == userId, cancellationToken);
                if (!ownsAddress)
                    throw new ForbiddenException("You do not own the selected address.");
            }

            var productIds = request.Items.Where(x => x.ProductId.HasValue).Select(x => x.ProductId!.Value).Distinct().ToList();
            var serviceIds = request.Items.Where(x => x.RepairServiceId.HasValue).Select(x => x.RepairServiceId!.Value).Distinct().ToList();
            var products = await _unitOfWork.ProductRepository.Find(x => productIds.Contains(x.Id) && x.IsActive).ToListAsync(cancellationToken);
            var services = await _unitOfWork.ServiceRepository.Find(x => serviceIds.Contains(x.Id) && x.IsActive).ToListAsync(cancellationToken);
            var productById = products.ToDictionary(x => x.Id);
            var serviceById = services.ToDictionary(x => x.Id);

            var order = new Order
            {
                UserId = userId,
                AddressId = request.AddressId,
                Note = request.Note,
                Status = OrderStatuses.PendingPayment,
                OrderDate = DateTime.UtcNow,
                IdempotencyKey = idempotencyKey,
                ShippingFee = 0
            };

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new BadRequestException("Quantity must be greater than zero.");
                }

                if (item.ProductId.HasValue == item.RepairServiceId.HasValue)
                {
                    throw new BadRequestException("Each order item must reference exactly one product or repair service.");
                }

                if (item.ProductId.HasValue)
                {
                    if (!productById.TryGetValue(item.ProductId.Value, out var product))
                        throw new NotFoundException(nameof(Product), item.ProductId.Value);
                    if (product.StockQuantity < item.Quantity)
                        throw new BadRequestException($"Insufficient stock for product {product.Name}.");

                    var subtotal = product.Price * item.Quantity;
                    order.OrderDetails.Add(new OrderDetail
                    {
                        Order = order,
                        Product = product,
                        ProductId = product.Id,
                        ProductNameSnapshot = product.Name,
                        SkuSnapshot = product.Sku,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = subtotal
                    });
                    await _unitOfWork.InventoryTransactionRepository.AddAsync(new InventoryTransaction
                    {
                        Product = product,
                        ProductId = product.Id,
                        Order = order,
                        Type = InventoryTransactionType.Reserved,
                        Quantity = item.Quantity,
                        Reference = idempotencyKey
                    });
                }
                else
                {
                    if (!serviceById.TryGetValue(item.RepairServiceId!.Value, out var service))
                        throw new NotFoundException(nameof(RepairService), item.RepairServiceId.Value);

                    order.OrderDetails.Add(new OrderDetail
                    {
                        Order = order,
                        RepairService = service,
                        RepairServiceId = service.Id,
                        ServiceNameSnapshot = service.Name,
                        Quantity = item.Quantity,
                        UnitPrice = service.BasePrice,
                        Subtotal = service.BasePrice * item.Quantity
                    });
                }
            }

            order.TotalAmount = order.OrderDetails.Sum(x => x.Subtotal) + order.ShippingFee - order.DiscountAmount;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            PaymentEntity? payment = null;

            try
            {
                await _unitOfWork.OrderRepository.AddAsync(order);
                foreach (var productGroup in order.OrderDetails
                    .Where(x => x.ProductId.HasValue)
                    .GroupBy(x => x.ProductId!.Value))
                {
                    if (!await _unitOfWork.ProductRepository.TryReserveStockAsync(
                        productGroup.Key,
                        productGroup.Sum(x => x.Quantity),
                        cancellationToken))
                    {
                        var productName = productGroup.First().ProductNameSnapshot ?? productGroup.Key.ToString();
                        throw new BadRequestException($"Insufficient stock for product {productName}.");
                    }

                    var detail = productGroup.First();
                    await _unitOfWork.StockReservationRepository.AddAsync(new StockReservation
                    {
                        ProductId = productGroup.Key,
                        Product = detail.Product!,
                        OrderId = order.Id,
                        Order = order,
                        Quantity = productGroup.Sum(x => x.Quantity),
                        ExpiresAt = DateTime.UtcNow.AddMinutes(30)
                    });
                }

                payment = PaymentEntity.Create(
                    order.Id,
                    order.TotalAmount,
                    $"ORDER_{order.Id:N}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    idempotencyKey);
                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

            try
            {
                var checkout = await _paymentGateway.CreateCheckoutAsync(
                    new PaymentCheckoutRequest(
                        payment.InvoiceNumber,
                        payment.Amount,
                        $"Thanh toán đơn hàng {order.Id}",
                        _paymentGateway.CheckoutUrls.SuccessUrl,
                        _paymentGateway.CheckoutUrls.ErrorUrl,
                        _paymentGateway.CheckoutUrls.CancelUrl),
                    cancellationToken);
                payment.SetCheckoutData(checkout.CheckoutUrl, checkout.Fields);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                if (checkoutCart != null)
                {
                    checkoutCart.Items.Clear();
                    checkoutCart.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            catch
            {
                payment.MarkAsFailed();
                await _inventoryService.ReleaseAsync(order.Id, "checkout-failed", cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw;
            }

            return new CreateOrderResponse
            {
                OrderId = order.Id,
                PaymentId = payment.Id,
                CheckoutUrl = payment.CheckoutUrl ?? string.Empty,
                Fields = payment.GetCheckoutFields()
            };
        }
    }
}
