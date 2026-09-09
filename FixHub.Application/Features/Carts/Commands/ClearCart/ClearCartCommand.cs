using FixHub.Application.Features.Carts.DTOs;
using MediatR;

namespace FixHub.Application.Features.Carts.Commands.ClearCart;

public sealed record ClearCartCommand : IRequest<CartResponse>;
