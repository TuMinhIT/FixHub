using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Carts.DTOs;
using FixHub.Application.Features.Carts.Services;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.Carts.Commands.UpdateCartItem;

public sealed class UpdateCartItemHandler : IRequestHandler<UpdateCartItemCommand, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCartItemHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CartResponse> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            throw new BadRequestException("Quantity must be greater than zero.");

        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cart), _currentUserService.UserId);
        var item = cart.Items.FirstOrDefault(x => x.Id == request.ItemId)
            ?? throw new NotFoundException(nameof(CartItem), request.ItemId);

        if (!item.Product.IsActive)
            throw new BadRequestException("Product is no longer available.");
        if (request.Quantity > item.Product.StockQuantity)
            throw new BadRequestException("Requested quantity exceeds available stock.");

        item.Quantity = request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CartResponseFactory.Create(cart);
    }
}
