using RookieShop.Shopping.Domain.Abstractions;
using RookieShop.Shopping.Domain.Carts.Events;
using RookieShop.Shopping.Domain.Shared;

namespace RookieShop.Shopping.Domain.Carts;

public class Cart : DomainEventSource
{
    public readonly Guid Id;

    public bool IsClosedForCheckout { get; private set; }

    private readonly List<CartItem> _items;
    public IReadOnlyList<CartItem> Items => _items;
    
#pragma warning disable CS8618, CS9264
    public Cart() {}
#pragma warning restore CS8618, CS9264

    public Cart(Guid id)
    {
        Id = id;
        IsClosedForCheckout = false;
        _items = [];
    }
    
    public decimal Total => Items.Sum(item => item.Subtotal);

    public void ThrowIfIsClosedForCheckout()
    {
        if (IsClosedForCheckout)
        {
            throw new InvalidOperationException("The cart is closed for checkout.");
        }
    }

    public int GetItemQuantity(string sku)
    {
        var item = Items.FirstOrDefault(item => item.Sku == sku);

        if (item == null)
        {
            throw new CartItemNotFoundException(Id, sku);
        }

        return item.Quantity;
    }

    public void AddItem(string sku, string name, decimal price, Guid imageId, int quantity)
    {
        ThrowIfIsClosedForCheckout();
        
        var item = _items.FirstOrDefault(item => item.Sku == sku);

        if (item == null)
        {
            item = new CartItem(sku, name, price, imageId, quantity);
            _items.Add(item);
        }
        else
        {
            item.Quantity += quantity;
        }
    }

    public void AdjustItemQuantity(string sku, int newQuantity)
    {
        ThrowIfIsClosedForCheckout();
        
        var item = _items.FirstOrDefault(item => item.Sku == sku);

        if (item == null)
        {
            throw new CartItemNotFoundException(Id, sku);
        }

        if (item.Quantity == newQuantity)
        {
            return;
        }
        
        var oldQuantity = item.Quantity;
        item.Quantity = newQuantity;

        if (newQuantity == 0)
        {
            _items.Remove(item);
        }
    }

    public void RemoveItem(string sku)
    {
        ThrowIfIsClosedForCheckout();
        
        var item = _items.FirstOrDefault(item => item.Sku == sku);

        if (item == null)
        {
            throw new CartItemNotFoundException(Id, sku);
        }

        _items.Remove(item);
    }
    
    private void InternalExpire()
    {
        AddDomainEvent(new CartExpired
        {
            Id = Id,
            Items = _items.Select(item => new CartExpired.Item
            {
                Sku = item.Sku,
                Quantity = item.Quantity,
            }).ToList()
        });
        
        _items.Clear();
    }

    public void Expire()
    {
        if (IsClosedForCheckout)
        {
            return;
        }

        InternalExpire();
    }

    public void StartCheckout()
    {
        ThrowIfIsClosedForCheckout();

        if (_items.Count == 0)
        {
            throw new InvalidOperationException("The cart is empty.");
        }
        
        IsClosedForCheckout = true;
        
        AddDomainEvent(new CartCheckoutStarted
        {
            Id = Id,
            Items = _items.Select(item => new CheckoutItem(item.Sku, item.Name, item.Price, item.Quantity))
        });
    }

    public void FailCheckout()
    {
        if (!IsClosedForCheckout)
        {
            throw new InvalidOperationException("The cart is not closed for checkout.");
        }
        
        IsClosedForCheckout = false;
        InternalExpire();
    }

    public void CompleteCheckout()
    {
        if (!IsClosedForCheckout)
        {
            throw new InvalidOperationException("The cart is not closed for checkout.");
        }
        
        IsClosedForCheckout = false;
        _items.Clear();
    }
}

public class CartItem
{
    public readonly string Sku;
    public readonly string Name;
    public readonly decimal Price;
    public readonly Guid ImageId;
    public int Quantity { get; internal set; }
    
#pragma warning disable CS8618, CS9264
    public CartItem() {}
#pragma warning restore CS8618, CS9264

    internal CartItem(string sku, string name, decimal price, Guid imageId, int quantity)
    {
        Sku = sku;
        Name = name;
        Price = price;
        ImageId = imageId;
        Quantity = quantity;
    }
    
    public decimal Subtotal => Price * Quantity;
}

public class CartItemNotFoundException : Exception
{
    public readonly Guid Id;
    public readonly string Sku;

    public CartItemNotFoundException(Guid id, string sku) : base($"Item {sku} was not found in cart {id}.")
    {
        Id = id;
        Sku = sku;
    }
}