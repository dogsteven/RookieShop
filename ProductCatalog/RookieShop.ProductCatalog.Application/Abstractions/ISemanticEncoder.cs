namespace RookieShop.ProductCatalog.Application.Abstractions;

public interface ISemanticEncoder
{
    public Task<float[]> EncodeAsync(string semantic, CancellationToken cancellationToken = default);
}