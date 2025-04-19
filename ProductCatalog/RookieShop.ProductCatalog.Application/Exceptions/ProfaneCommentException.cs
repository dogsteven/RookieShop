namespace RookieShop.ProductCatalog.Application.Exceptions;

public class ProfaneCommentException : Exception
{
    public ProfaneCommentException() : base("Profane comments are not allowed.") {}
}