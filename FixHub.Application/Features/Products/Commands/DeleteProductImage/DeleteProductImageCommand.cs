using MediatR;

namespace FixHub.Application.Features.Products.Commands.DeleteProductImage;

public sealed record DeleteProductImageCommand(Guid ProductId, Guid ImageId) : IRequest<bool>;
