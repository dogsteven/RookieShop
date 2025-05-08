namespace RookieShop.Shopping.Domain.Shared;

public class CheckoutItem
{
    public readonly string Sku;
    public readonly string Name;
    public readonly decimal Price;
    public readonly int Quantity;
    
#pragma warning disable CS8618, CS9264
    public CheckoutItem() {}
#pragma warning restore CS8618, CS9264

    public CheckoutItem(string sku, string name, decimal price, int quantity)
    {
        Sku = sku;
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public CheckoutItem Clone()
    {
        return new CheckoutItem(Sku, Name, Price, Quantity);
    }
}