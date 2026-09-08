using FixHub.Application.Common.Models;
using FixHub.Infrastructure.Payment;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/sepay")]
    [Route("api/v1/payments/sepay")]
    public class SePayIPNendpoint : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderInventoryService _inventoryService;

        public SePayIPNendpoint(IConfiguration configuration, IUnitOfWork unitOfWork, IOrderInventoryService inventoryService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
        }

        [HttpPost("ipn")]
        public async Task<IActionResult> ReceiveIpn([FromForm] Dictionary<string, string> formData, CancellationToken cancellationToken)
        {
            if (formData.Count == 0)
            {
                return BadRequest(new ApiResponse<string>(null!, "No payload received") { Success = false });
            }

            var secretKey = _configuration["SePay:SecretKey"] ?? string.Empty;
            var providedSignature = formData.GetValueOrDefault("signature");

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(providedSignature))
            {
                return BadRequest(new ApiResponse<string>(null!, "Invalid SePay callback") { Success = false });
            }

            var expectedSignature = SePaySignatureService.Generate(formData, secretKey);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedSignature);
            var providedBytes = Encoding.UTF8.GetBytes(providedSignature);
            if (expectedBytes.Length != providedBytes.Length
                || !CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes))
            {
                return BadRequest(new ApiResponse<string>(null!, "SePay signature mismatch") { Success = false });
            }

            var orderIdRaw = formData.GetValueOrDefault("order_id") ?? formData.GetValueOrDefault("order_invoice_number");
            if (string.IsNullOrWhiteSpace(orderIdRaw))
            {
                return BadRequest(new ApiResponse<string>(null!, "Missing order reference") { Success = false });
            }

            var payment = await _unitOfWork.PaymentRepository.Find(x => x.InvoiceNumber == orderIdRaw)
                .FirstOrDefaultAsync(cancellationToken);
            if (payment == null && Guid.TryParse(orderIdRaw, out var orderId))
            {
                payment = await _unitOfWork.PaymentRepository.Find(x => x.OrderId == orderId)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            if (payment == null)
                return NotFound(new ApiResponse<string>(null!, "Payment not found") { Success = false });

            var transactionId = formData.GetValueOrDefault("transaction_id")
                ?? formData.GetValueOrDefault("reference_number")
                ?? formData.GetValueOrDefault("id")
                ?? $"{orderIdRaw}:{providedSignature}";
            var existingEvent = await _unitOfWork.PaymentEventRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Provider == "SePay" && x.ProviderEventId == transactionId, cancellationToken);
            if (existingEvent != null)
                return Ok(new ApiResponse<string>("OK", "Payment callback already processed"));

            if (!formData.TryGetValue("order_amount", out var amountRaw)
                || !decimal.TryParse(amountRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount)
                || amount != payment.Amount)
            {
                return BadRequest(new ApiResponse<string>(null!, "Payment amount mismatch") { Success = false });
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
            var callbackStatus = formData.GetValueOrDefault("status")
                ?? formData.GetValueOrDefault("transaction_status")
                ?? formData.GetValueOrDefault("payment_status");
            var isSuccess = string.IsNullOrWhiteSpace(callbackStatus)
                || new[] { "success", "paid", "completed", "1", "true" }
                    .Contains(callbackStatus.Trim().ToLowerInvariant());
            if (isSuccess)
                payment.MarkAsPaid(transactionId);
            else
                payment.MarkAsFailed();

            var order = await _unitOfWork.OrderRepository.FindById(payment.OrderId)
                ?? throw new NotFoundException(nameof(Order), payment.OrderId);
            if (isSuccess && OrderStatuses.CanTransition(order.Status, OrderStatuses.Paid))
            {
                order.Status = OrderStatuses.Paid;
                await _inventoryService.FinalizeAsync(order.Id, transactionId, cancellationToken);
            }

            var payloadJson = JsonSerializer.Serialize(formData.OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value));
            var payloadHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payloadJson))).ToLowerInvariant();
            await _unitOfWork.PaymentEventRepository.AddAsync(new PaymentEvent
            {
                PaymentId = payment.Id,
                Provider = "SePay",
                ProviderEventId = transactionId,
                PayloadHash = payloadHash,
                PayloadJson = payloadJson,
                ProcessedAt = DateTime.UtcNow,
                Status = isSuccess ? "Processed" : "Failed"
            });
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

            return Ok(new ApiResponse<string>("OK", "Payment callback processed successfully"));
        }
    }
}
