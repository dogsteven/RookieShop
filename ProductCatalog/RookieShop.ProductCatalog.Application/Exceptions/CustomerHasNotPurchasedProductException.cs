namespace RookieShop.ProductCatalog.Application.Exceptions;

public class CustomerHasNotPurchasedProductException : Exception
{
    public readonly Guid CustomerId;
    public readonly string ProductSku;
    
    public CustomerHasNotPurchasedProductException(Guid customerId, string productSku) : base($"Customer {customerId} has not purchased product {productSku}")
    {
        CustomerId = customerId;
        ProductSku = productSku;
    }
}