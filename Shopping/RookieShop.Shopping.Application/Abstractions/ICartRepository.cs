using RookieShop.Shopping.Domain;

namespace RookieShop.Shopping.Application.Abstractions;

public interface ICartRepository
{
    public Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public void Save(Cart cart);
    public void Remove(Cart cart);
}