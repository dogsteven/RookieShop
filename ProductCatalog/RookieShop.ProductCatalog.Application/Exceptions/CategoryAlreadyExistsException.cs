namespace RookieShop.ProductCatalog.Application.Exceptions;

public class CategoryAlreadyExistsException : Exception
{
    public readonly string Name;

    public CategoryAlreadyExistsException(string name) : base($"Category \"{name}\" already exists.")
    {
        Name = name;
    }
}