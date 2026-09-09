using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Carts.DTOs;
using FixHub.Application.Features.Carts.Services;
using MediatR;

namespace FixHub.Application.Features.Carts.Commands.ClearCart;

public sealed class ClearCartHandler : IRequestHandler<ClearCartCommand, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ClearCartHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CartResponse> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
        if (cart == null)
        {
            cart = new Domain.Entities.Cart { UserId = _currentUserService.UserId };
            await _unitOfWork.CartRepository.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return CartResponseFactory.Create(cart);
        }
        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.CartRepository.UpdateAsync(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CartResponseFactory.Create(cart);
    }
}
