using FixHub.Application.Common.Models;
using FixHub.Application.Payment.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController(IMediator _mediator) : ControllerBase
    {
        [HttpPost("{orderId:guid}/checkout")]
        [Authorize]
        public async Task<IActionResult> CreatePayment(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CreatePaymentCommand(orderId),
                cancellationToken);

            return Ok(new ApiResponse<CreatePaymentResponse>(result, "SePay checkout created successfully"));
        }
    }
}
