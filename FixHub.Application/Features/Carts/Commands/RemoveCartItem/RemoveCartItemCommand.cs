using FixHub.Application.Features.Carts.DTOs;
using MediatR;

namespace FixHub.Application.Features.Carts.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(Guid ItemId) : IRequest<CartResponse>;
