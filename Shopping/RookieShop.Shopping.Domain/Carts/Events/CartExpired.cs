namespace RookieShop.Shopping.Domain.Carts.Events;

public class CartExpired
{
    public Guid Id { get; init; }

    public IEnumerable<Item> Items { get; init; } = null!;
    
    public class Item
    {
        public string Sku { get; init; } = null!;
        
        public int Quantity { get; init; }
    }
}