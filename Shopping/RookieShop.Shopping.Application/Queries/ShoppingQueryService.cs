using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Models;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain;

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
            Items = cart.Items.Select(item => new CartItemDto
            {
                Sku = item.Sku,
                Name = item.Name,
                Price = item.Price,
                ImageId = item.ImageId,
                Quantity = item.Quantity
            })
        };
    }

    public async Task<CartDto> GetCartByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(id, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);

        return Map(cart);
    }
}