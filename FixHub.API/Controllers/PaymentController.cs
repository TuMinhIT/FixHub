using FixHub.Application.Common.Models;
using FixHub.Application.Payment.Command;
using FixHub.Application.Payment.Queries.GetPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Route("api/v1/payments")]
    public class PaymentController(IMediator _mediator) : ControllerBase
    {
        [HttpGet("{paymentId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetPayment(Guid paymentId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPaymentQuery(paymentId), cancellationToken);
            return Ok(new ApiResponse<PaymentResponse>(result));
        }

        [HttpPost("{orderId:guid}/checkout")]
        [Authorize]
        public async Task<IActionResult> CreatePayment(
            Guid orderId,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CreatePaymentCommand(orderId, idempotencyKey),
                cancellationToken);

            return Ok(new ApiResponse<CreatePaymentResponse>(result, "SePay checkout created successfully"));
        }
    }
}
