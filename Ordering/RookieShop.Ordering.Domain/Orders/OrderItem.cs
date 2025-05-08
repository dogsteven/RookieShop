namespace RookieShop.Ordering.Domain.Orders;

public class OrderItem
{
    public string Sku { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }

#pragma warning disable CS8618, CS9264
    public OrderItem() {}
#pragma warning restore CS8618, CS9264
    
    public OrderItem(string sku, string name, decimal price, int quantity)
    {
        Sku = sku;
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}