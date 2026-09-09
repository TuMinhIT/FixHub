using FixHub.Application.Features.Carts.DTOs;
using MediatR;

namespace FixHub.Application.Features.Carts.Queries.GetCart;

public sealed record GetCartQuery : IRequest<CartResponse>;
