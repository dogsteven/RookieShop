namespace RookieShop.ProductReview.Application.Abstractions;

public interface IProfanityChecker
{
    public ValueTask<bool> CheckProfanityAsync(string text, CancellationToken cancellationToken);
}