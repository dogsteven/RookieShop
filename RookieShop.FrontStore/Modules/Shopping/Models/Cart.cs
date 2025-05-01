namespace RookieShop.FrontStore.Modules.Shopping.Models;

public class Cart
{
    public Guid Id { get; init; }

    public IEnumerable<CartItem> Items { get; init; } = null!;
    
    public decimal Total { get; init; }
}

public class CartItem
{
    public string Sku { get; init; } = null!;
    
    public string Name { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public Guid ImageId { get; init; }
    
    public int Quantity { get; init; }
    
    public decimal Subtotal { get; init; }
}