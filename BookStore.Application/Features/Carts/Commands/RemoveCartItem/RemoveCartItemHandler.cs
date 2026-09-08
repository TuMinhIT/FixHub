using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Carts.DTOs;
using FixHub.Application.Features.Carts.Services;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.Carts.Commands.RemoveCartItem;

public sealed class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RemoveCartItemHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CartResponse> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cart), _currentUserService.UserId);
        var item = cart.Items.FirstOrDefault(x => x.Id == request.ItemId)
            ?? throw new NotFoundException(nameof(CartItem), request.ItemId);

        cart.Items.Remove(item);
        await _unitOfWork.CartRepository.UpdateAsync(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CartResponseFactory.Create(cart);
    }
}
