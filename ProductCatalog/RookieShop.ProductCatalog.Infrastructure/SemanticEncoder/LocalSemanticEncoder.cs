using AllMiniLmL6V2Sharp;
using RookieShop.ProductCatalog.Application.Abstractions;

namespace RookieShop.ProductCatalog.Infrastructure.SemanticEncoder;

public class LocalSemanticEncoder : ISemanticEncoder
{
    public Task<float[]> EncodeAsync(string semantic, CancellationToken cancellationToken = default)
    {
        using var embedder = new AllMiniLmL6V2Embedder();

        var embedding = embedder.GenerateEmbedding(semantic);

        return Task.FromResult(embedding.ToArray());
    }
}