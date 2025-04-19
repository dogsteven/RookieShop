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

        var rating = await _dbContext.Ratings
            .FirstOrDefaultAsync(rating => rating.ProductSku == sku, cancellationToken);

        if (rating == null)
        {
            return;
        }

        rating.ApplyScore(score);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}