namespace RookieShop.ProductCatalog.Application.Exceptions;

public class CustomerHasNotWrittenReviewException : Exception
{
    public CustomerHasNotWrittenReviewException(Guid writerId, string productSku)
        : base($"Customer with id \"{writerId}\" hasn't written a review for product {productSku} yet.") {}
}