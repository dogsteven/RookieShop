namespace RookieShop.ProductReview.Application.Exceptions;

public class ProfaneCommentException : Exception
{
    public ProfaneCommentException() : base("Profane comment is not allowed.") {}
}