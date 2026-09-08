using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Carts.DTOs;
using FixHub.Application.Features.Carts.Services;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Carts.Commands.AddCartItem;

public sealed class AddCartItemHandler : IRequestHandler<AddCartItemCommand, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddCartItemHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CartResponse> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            throw new BadRequestException("Quantity must be greater than zero.");

        var product = await _unitOfWork.ProductRepository.GetAll()
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.IsActive, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.ProductId);

        if (request.Quantity > product.StockQuantity)
            throw new BadRequestException("Requested quantity exceeds available stock.");

        var cart = await _unitOfWork.CartRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
        var isNewCart = cart == null;
        cart ??= new Cart { UserId = _currentUserService.UserId };

        var item = cart.Items.FirstOrDefault(x => x.ProductId == product.Id);
        if (item == null)
        {
            item = new CartItem { Cart = cart, Product = product, ProductId = product.Id, Quantity = request.Quantity };
            cart.Items.Add(item);
        }
        else
        {
            item.Quantity += request.Quantity;
            if (item.Quantity > product.StockQuantity)
                throw new BadRequestException("Requested quantity exceeds available stock.");
            item.UpdatedAt = DateTime.UtcNow;
        }

        cart.UpdatedAt = DateTime.UtcNow;
        if (isNewCart)
            await _unitOfWork.CartRepository.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CartResponseFactory.Create(cart);
    }
}
