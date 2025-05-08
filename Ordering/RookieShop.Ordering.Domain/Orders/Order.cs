using RookieShop.Ordering.Domain.Abstractions;
using RookieShop.Ordering.Domain.Orders.Events;
using RookieShop.Shared.Models;

namespace RookieShop.Ordering.Domain.Orders;

public class Order : DomainEventSource
{
    public readonly Guid Id;
    
    public readonly Guid CustomerId;

    public readonly DateTimeOffset PlacedTime;

    public readonly Address BillingAddress;
    
    public readonly Address ShippingAddress;
    
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items;
    public IEnumerable<OrderItem> Items => _items;
    
#pragma warning disable CS8618, CS9264
    public Order() {}
#pragma warning restore CS8618, CS9264

    public Order(Guid id, Guid customerId, Address billingAddress, Address shippingAddress, IEnumerable<OrderItem> items, TimeProvider timeProvider)
    {
        Id = id;
        CustomerId = customerId;
        PlacedTime = timeProvider.GetUtcNow();
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Placed;
        _items = items.ToList();
        
        AddDomainEvent(new OrderPlaced
        {
            Id = Id,
            CustomerId = customerId,
        });
    }

    public void Cancel(Guid userId)
    {
        if (userId != CustomerId)
        {
            throw new InvalidOperationException("Only the owner of this order is allowed to cancel it.");
        }
        
        if (Status != OrderStatus.Placed)
        {
            throw new InvalidOperationException($"Order {Id} has been cancelled or completed.");
        }
        
        Status = OrderStatus.Cancelled;
        
        AddDomainEvent(new OrderCancelled
        {
            Id = Id,
            CustomerId = CustomerId,
            Items = _items
        });
    }

    public void Complete()
    {
        if (Status != OrderStatus.Placed)
        {
            throw new InvalidOperationException($"Order {Id} has been cancelled or completed.");
        }
        
        Status = OrderStatus.Completed;
        
        AddDomainEvent(new OrderCompleted
        {
            Id = Id,
            CustomerId = CustomerId,
            Items = _items
        });
    }
}

public enum OrderStatus
{
    Placed, Cancelled, Completed
}