namespace RookieShop.ProductCatalog.Application.Exceptions;

public class MakeReactionToOwnReviewException : Exception
{
    public MakeReactionToOwnReviewException() : base("Don't allow to make reaction to own review.") {}
}