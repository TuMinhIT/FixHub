using FixHub.Application.Common.Models;
using FixHub.Application.Features.Orders.Commands.CreateOrder;
using FixHub.Application.Features.Orders.Commands.DeleteOrder;
using FixHub.Application.Features.Orders.Commands.CancelOrder;
using FixHub.Application.Features.Orders.Commands.UpdateOrderStatus;
using FixHub.Application.Features.Orders.DTOs;
using FixHub.Application.Features.Orders.Queries.GetAllOrders;
using FixHub.Application.Features.Orders.Queries.GetMyOrders;
using FixHub.Application.Features.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllOrdersQuery(), cancellationToken);
            return Ok(new ApiResponse<List<OrderResponse>>(response));
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
            return Ok(new ApiResponse<OrderResponse>(response));
        }
        
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders(Guid userId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetMyOrdersQuery(userId), cancellationToken);
            return Ok(new ApiResponse<List<OrderResponse>>(response));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetMyOrdersQuery(Guid.Empty), cancellationToken);
            return Ok(new ApiResponse<List<OrderResponse>>(response));
        }

        [HttpGet("/api/v1/me/orders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrdersV1(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetMyOrdersQuery(Guid.Empty), cancellationToken);
            return Ok(new ApiResponse<List<OrderResponse>>(response));
        }

        [HttpGet("/api/v1/me/orders/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetMyOrderByIdV1(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
            return Ok(new ApiResponse<OrderResponse>(response));
        }

        [HttpPost("{id:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CancelOrderCommand(id), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Order cancelled successfully"));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderCommand command,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            CancellationToken cancellationToken)
        {
            command.IdempotencyKey ??= idempotencyKey;

            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<CreateOrderResponse>(response, "Order created successfully"));
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Order status updated successfully"));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(Guid id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DeleteOrderCommand(id), cancellationToken);
            return Ok(new ApiResponse<bool>(response, "Order deleted successfully"));
        }
    }
}
