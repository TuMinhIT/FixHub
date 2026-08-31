//using FixHub.Application.Common.Interfaces;
//using FixHub.Domain.IRepositories;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FixHub.Application.Payment.Command
//{
//    public sealed class CreatePaymentCommandHandler
//    : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
//    {
//        private readonly IOrderRepository _orderRepository;
//        private readonly IPaymentRepository _paymentRepository;
//        private readonly IPaymentRepository _paymentGateway;
//        private readonly IUnitOfWork _unitOfWork;

//        public CreatePaymentCommandHandler(
//            IOrderRepository orderRepository,
//            IPaymentRepository paymentRepository,
//            IPaymentRepository paymentGateway,
//            IUnitOfWork unitOfWork)
//        {
//            _orderRepository = orderRepository;
//            _paymentRepository = paymentRepository;
//            _paymentGateway = paymentGateway;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<CreatePaymentResult> Handle(
//            CreatePaymentCommand request,
//            CancellationToken cancellationToken)
//        {
//            var order = await _orderRepository.GetByIdAsync(
//                request.OrderId,
//                cancellationToken);

//            if (order is null)
//                throw new Exception("Order not found");

//            if (order.Status != OrderStatus.PendingPayment)
//                throw new Exception("Order cannot be paid");

//            var invoiceNumber =
//                $"ORDER_{order.Id:N}_{DateTime.UtcNow:yyyyMMddHHmmss}";

//            var payment = Payment.Create(
//                order.Id,
//                order.TotalAmount,
//                invoiceNumber);

//            await _paymentRepository.AddAsync(
//                payment,
//                cancellationToken);

//            var checkout = await _paymentGateway.CreateCheckoutAsync(
//                new PaymentCheckoutRequest(
//                    InvoiceNumber: invoiceNumber,
//                    Amount: payment.Amount,
//                    Description: $"Thanh toan don hang {order.Id}",
//                    SuccessUrl: "https://yourfrontend.com/payment/success",
//                    ErrorUrl: "https://yourfrontend.com/payment/error",
//                    CancelUrl: "https://yourfrontend.com/payment/cancel"),
//                cancellationToken);

//            await _unitOfWork.SaveChangesAsync(cancellationToken);

//            return new CreatePaymentResult(
//                payment.Id,
//                checkout.CheckoutUrl,
//                checkout.Fields);
//        }
//    }
//}
