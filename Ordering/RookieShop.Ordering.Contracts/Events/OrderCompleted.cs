namespace RookieShop.Ordering.Contracts.Events;

public class OrderCompleted
{
    public Guid Id { get; init; }
    
    public Guid CustomerId { get; init; }

    public IEnumerable<Item> Items { get; init; } = null!;
    
    public class Item
    {
        public readonly string Sku;
        public readonly int Quantity;

#pragma warning disable CS8618, CS9264
        public Item() {}
#pragma warning restore CS8618, CS9264
    
        public Item(string sku, int quantity)
        {
            Sku = sku;
            Quantity = quantity;
        }
    }
}