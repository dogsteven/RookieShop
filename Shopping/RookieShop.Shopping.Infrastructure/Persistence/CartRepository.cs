using Microsoft.EntityFrameworkCore;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Domain;

namespace RookieShop.Shopping.Infrastructure.Persistence;

public class CartRepository : ICartRepository
{
    private readonly ShoppingDbContext _context;

    public CartRepository(ShoppingDbContext context)
    {
        _context = context;
    }
    
    public Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Carts.FirstOrDefaultAsync(cart => cart.Id == id, cancellationToken);
    }

    public void Save(Cart cart)
    {
        var entry = _context.Entry(cart);

        if (entry.State == EntityState.Detached)
        {
            _context.Carts.Add(cart);
        }
        else if (entry.State == EntityState.Unchanged)
        {
            _context.Carts.Update(cart);
        }
    }
}