namespace RookieShop.ProductCatalog.Application.Exceptions;

public class CustomerHasAlreadyWrittenReviewException : Exception
{
    public readonly Guid WriterId;
    public readonly string ProductSku;

    public CustomerHasAlreadyWrittenReviewException(Guid writerId, string productSku)
        : base($"Customer \"{writerId}\" has already written review for product {productSku}.")
    {
        WriterId = writerId;
        ProductSku = productSku;
    }
}