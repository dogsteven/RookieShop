namespace RookieShop.Ordering.Domain.Orders;

public class OrderItem
{
    public readonly string Sku;
    public readonly string Name;
    public readonly decimal Price;
    public readonly int Quantity;

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