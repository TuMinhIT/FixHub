using FixHub.Application.Features.Carts.DTOs;
using MediatR;

namespace FixHub.Application.Features.Carts.Commands.UpdateCartItem;

public sealed record UpdateCartItemCommand(Guid ItemId, int Quantity) : IRequest<CartResponse>;
