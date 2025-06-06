using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Carts;
using RookieShop.Shopping.ViewModels;

namespace RookieShop.Shopping.Application.Queries;

public class ShoppingQueryService
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingQueryService(CartRepositoryHelper cartRepositoryHelper, IUnitOfWork unitOfWork)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _unitOfWork = unitOfWork;
    }

    private static CartDto Map(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            IsClosedForCheckout = cart.IsClosedForCheckout,
            Items = cart.Items.Select(item => new CartItemDto
            {
                Sku = item.Sku,
                Name = item.Name,
                Price = item.Price,
                ImageId = item.ImageId,
                Quantity = item.Quantity,
                Subtotal = item.Subtotal
            }),
            Total = cart.Total
        };
    }

    public async Task<CartDto> GetCartByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(id, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);

        return Map(cart);
    }
}