using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;

namespace RookieShop.ProductCatalog.Application.Events;

public class ReviewSubmitted
{
    public string ProductSku { get; set; } = null!;
    
    public int Score { get; set; }
}

public class ApplyScoreConsumer : IConsumer<ReviewSubmitted>
{
    private readonly ProductCatalogDbContext _dbContext;

    public ApplyScoreConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ReviewSubmitted> context)
    {
        var message = context.Message;
        var sku = message.ProductSku;
        var score = message.Score;

        var cancellationToken = context.CancellationToken;

        var productRating = await _dbContext.ProductRatings
            .FirstOrDefaultAsync(productRating => productRating.ProductSku == sku, cancellationToken);

        if (productRating == null)
        {
            return;
        }

        productRating.ApplyScore(score);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}