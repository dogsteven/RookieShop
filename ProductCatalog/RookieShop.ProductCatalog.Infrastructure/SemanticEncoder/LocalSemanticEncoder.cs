using AllMiniLmL6V2Sharp;
using Microsoft.Extensions.Logging;
using RookieShop.ProductCatalog.Application.Abstractions;

namespace RookieShop.ProductCatalog.Infrastructure.SemanticEncoder;

public class LocalSemanticEncoder : ISemanticEncoder
{
    private readonly ILogger<LocalSemanticEncoder> _logger;

    public LocalSemanticEncoder(ILogger<LocalSemanticEncoder> logger)
    {
        _logger = logger;
    }
    
    public Task<float[]> EncodeAsync(string semantic, CancellationToken cancellationToken = default)
    {
        using var embedder = new AllMiniLmL6V2Embedder();

        var embedding = embedder.GenerateEmbedding(semantic).ToArray();
        
        return Task.FromResult(embedding);
    }
}