namespace RookieShop.ProductCatalog.Application.Exceptions;

public class CustomerHasNotWrittenReviewException : Exception
{
    public readonly Guid WriterId;
    public readonly string ProductSku;

    public CustomerHasNotWrittenReviewException(Guid writerId, string productSku)
        : base($"Customer \"{writerId}\" hasn't written a review for product {productSku} yet.")
    {
        WriterId = writerId;
        ProductSku = productSku;
    }
}