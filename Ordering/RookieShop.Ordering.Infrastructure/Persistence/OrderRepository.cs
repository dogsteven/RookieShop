using RookieShop.Ordering.Application.Abstractions;
using RookieShop.Ordering.Domain.Orders;

namespace RookieShop.Ordering.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _context;

    public OrderRepository(OrderingDbContext context)
    {
        _context = context;
    }
    
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.FindAsync([id], cancellationToken);
    }

    public void Add(Order order)
    {
        _context.Orders.Add(order);
    }
}