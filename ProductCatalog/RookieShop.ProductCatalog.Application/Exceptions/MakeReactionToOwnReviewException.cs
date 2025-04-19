namespace RookieShop.ProductCatalog.Application.Exceptions;

public class MakeReactionToOwnReviewException : Exception
{
    public MakeReactionToOwnReviewException() : base("You aren't allowed to make reaction to your own reviews.") {}
}