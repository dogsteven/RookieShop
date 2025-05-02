using RookieShop.Shopping.Domain;
using RookieShop.Shopping.Domain.Carts;

namespace RookieShop.Shopping.Application.Abstractions.Repositories;

public interface ICartRepository
{
    public Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public void Save(Cart cart);
}