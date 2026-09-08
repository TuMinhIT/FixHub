using FixHub.Application.Features.Carts.DTOs;
using MediatR;

namespace FixHub.Application.Features.Carts.Commands.AddCartItem;

public sealed record AddCartItemCommand(Guid ProductId, int Quantity = 1) : IRequest<CartResponse>;
