using RookieShop.Shared.Models;
using RookieShop.Shopping.Domain.Abstractions;
using RookieShop.Shopping.Domain.CheckoutSessions.Events;
using RookieShop.Shopping.Domain.Shared;

namespace RookieShop.Shopping.Domain.CheckoutSessions;

public class CheckoutSession : DomainEventSource
{
    public Guid Id { get; init; }
    
    public bool IsActive { get; private set; }
    
    public Address? BillingAddress { get; private set; }
    
    public Address? ShippingAddress { get; private set; }

    private readonly List<CheckoutItem> _items;
    public IEnumerable<CheckoutItem> Items => _items;
    
#pragma warning disable CS8618, CS9264
    public CheckoutSession() {}
#pragma warning restore CS8618, CS9264

    public CheckoutSession(Guid id)
    {
        Id = id;
        IsActive = false;
        BillingAddress = null;
        ShippingAddress = null;
        _items = [];
    }

    public void Start()
    {
        if (IsActive)
        {
            throw new InvalidOperationException("Session is already active.");
        }
        
        IsActive = true;
        
        AddDomainEvent(new CheckoutSessionStarted
        {
            Id = Id,
        });
    }

    public void SetAddresses(Address billingAddress, Address shippingAddress)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Session is not active.");
        }
        
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
    }

    public void AddItems(IEnumerable<CheckoutItem> items)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Session is not active.");
        }
        
        _items.AddRange(items);
    }

    private void InternalExpire()
    {
        _items.Clear();
        
        AddDomainEvent(new CheckoutSessionExpired
        {
            Id = Id,
        });
    }

    public void Expire()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        InternalExpire();
    }

    public void Complete()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Session is not active.");
        }

        if (BillingAddress == null || ShippingAddress == null || _items.Count == 0)
        {
            throw new InvalidOperationException("Cannot complete this session.");
        }
        
        IsActive = false;
        
        AddDomainEvent(new CheckoutSessionCompleted
        {
            Id = Id,
            BillingAddress = BillingAddress,
            ShippingAddress = ShippingAddress,
            Items = _items.Select(item => item.Clone()).ToList()
        });
        
        _items.Clear();
    }
}