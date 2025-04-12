namespace RookieShop.Application.Exceptions;

public class CustomerHasNotPurchasedException : Exception
{
    public CustomerHasNotPurchasedException() : base("Customer has not purchased the product.") {}
}