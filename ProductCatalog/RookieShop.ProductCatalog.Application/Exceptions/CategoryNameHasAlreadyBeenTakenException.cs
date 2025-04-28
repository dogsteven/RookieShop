namespace RookieShop.ProductCatalog.Application.Exceptions;

public class CategoryNameHasAlreadyBeenTakenException : Exception
{
    public readonly string Name;

    public CategoryNameHasAlreadyBeenTakenException(string name) : base($"Category \"{name}\" has already been taken.")
    {
        Name = name;
    }
}