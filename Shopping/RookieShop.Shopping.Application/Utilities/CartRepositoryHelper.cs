using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Domain;

namespace RookieShop.Shopping.Application.Utilities;

public class CartRepositoryHelper
{
    private readonly ICartRepository _cartRepository;

    public CartRepositoryHelper(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Cart> GetOrCreateCartAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByIdAsync(id, cancellationToken);

        if (cart != null)
            return cart;
        
        cart = new Cart(id);
            
        _cartRepository.Save(cart);

        return cart;
    }
}