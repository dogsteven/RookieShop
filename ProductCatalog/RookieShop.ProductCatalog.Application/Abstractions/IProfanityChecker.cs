namespace RookieShop.ProductCatalog.Application.Abstractions;

public interface IProfanityChecker
{
    public ValueTask<bool> CheckProfanityAsync(string text, CancellationToken cancellationToken);
}