using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductReview.Contracts.Events;

namespace RookieShop.ProductCatalog.Application.EventConsumers;

public class ApplyRatingConsumer : IConsumer<ReviewWrote>
{
    private readonly ProductCatalogDbContext _dbContext;

    public ApplyRatingConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<ReviewWrote> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;
        
        var rating = await _dbContext.ProductRatings
            .FirstOrDefaultAsync(rating => rating.Sku == message.ProductSku, cancellationToken);

        if (rating == null)
        {
            return;
        }

        rating.ApplyScore(message.Score);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}