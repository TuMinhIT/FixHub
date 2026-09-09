using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Carts.DTOs;
using FixHub.Application.Features.Carts.Services;
using FixHub.Domain.Entities;
using MediatR;

namespace FixHub.Application.Features.Carts.Queries.GetCart;

public sealed class GetCartHandler : IRequestHandler<GetCartQuery, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetCartHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CartResponse> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
        if (cart != null)
            return CartResponseFactory.Create(cart);

        cart = new Cart { UserId = _currentUserService.UserId };
        await _unitOfWork.CartRepository.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CartResponseFactory.Create(cart);
    }
}
