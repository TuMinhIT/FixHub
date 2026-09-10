using FixHub.Application.Common.Models;
using FixHub.Application.Features.Carts.Commands.AddCartItem;
using FixHub.Application.Features.Carts.Commands.ClearCart;
using FixHub.Application.Features.Carts.Commands.RemoveCartItem;
using FixHub.Application.Features.Carts.Commands.UpdateCartItem;
using FixHub.Application.Features.Carts.DTOs;
using FixHub.Application.Features.Carts.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers;

[ApiController]
[Route("api/v1/cart")]
[Authorize]
public sealed class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) =>
        Ok(new ApiResponse<CartResponse>(await _mediator.Send(new GetCartQuery(), cancellationToken)));

    [HttpPost("items")]
    public async Task<IActionResult> Add([FromBody] AddCartItemCommand command, CancellationToken cancellationToken) =>
        Ok(new ApiResponse<CartResponse>(await _mediator.Send(command, cancellationToken), "Cart item added"));

    [HttpPatch("items/{itemId:guid}")]
    public async Task<IActionResult> Update(Guid itemId, [FromBody] UpdateCartItemRequest request, CancellationToken cancellationToken) =>
        Ok(new ApiResponse<CartResponse>(await _mediator.Send(new UpdateCartItemCommand(itemId, request.Quantity), cancellationToken), "Cart updated"));

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> Remove(Guid itemId, CancellationToken cancellationToken) =>
        Ok(new ApiResponse<CartResponse>(await _mediator.Send(new RemoveCartItemCommand(itemId), cancellationToken), "Cart item removed"));

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken) =>
        Ok(new ApiResponse<CartResponse>(await _mediator.Send(new ClearCartCommand(), cancellationToken), "Cart cleared"));
}

public sealed class UpdateCartItemRequest
{
    public int Quantity { get; set; }
}
