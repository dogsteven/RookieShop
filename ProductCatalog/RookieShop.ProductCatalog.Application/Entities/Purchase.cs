namespace RookieShop.ProductCatalog.Application.Entities;

public class Purchase
{
    public Guid CustomerId { get; set; }
    
    public string ProductSku { get; set; }
    
#pragma warning disable CS8618, CS9264
    public Purchase() {}
#pragma warning restore CS8618, CS9264
}