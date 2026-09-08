using MediatR;

namespace FixHub.Application.Features.Products.Commands.AddProductImage;

public sealed record AddProductImageCommand(
    Guid ProductId,
    string ImageUrl,
    string? PublicId = null,
    bool IsPrimary = false) : IRequest<Guid>;
