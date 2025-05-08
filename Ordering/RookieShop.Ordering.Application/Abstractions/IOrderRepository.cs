using RookieShop.Ordering.Domain.Orders;

namespace RookieShop.Ordering.Application.Abstractions;

public interface IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public void Add(Order order);
}