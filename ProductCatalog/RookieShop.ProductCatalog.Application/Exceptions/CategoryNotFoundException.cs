namespace RookieShop.ProductCatalog.Application.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(int id) : base($"Category with id {id} was not found.") {}
}