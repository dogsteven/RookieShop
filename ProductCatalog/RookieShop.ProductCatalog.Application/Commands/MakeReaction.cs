using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.ProductReview.Application.Commands;

public class MakeReaction
{
    public Guid ReactorId { get; set; }
    
    public Guid WriterId { get; set; }

    public string ProductSku { get; set; } = null!;
    
    public ReviewReactionType Type { get; set; }
}

public class MakeReactionConsumer : IConsumer<MakeReaction>
{
    private readonly ProductCatalogDbContext _dbContext;

    public MakeReactionConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<MakeReaction> context)
    {
        var message = context.Message;
        var reactorId = message.ReactorId;
        var writerId = message.WriterId;
        var productSku = message.ProductSku;
        var type = message.Type;
        
        var cancellationToken = context.CancellationToken;
        
        var customerHasWrittenReview = await _dbContext.Reviews
            .AnyAsync(review => review.WriterId == writerId && review.ProductSku == productSku, cancellationToken);

        if (!customerHasWrittenReview)
        {
            throw new CustomerHasNotWrittenReviewException(writerId, productSku);
        }

        if (reactorId == writerId)
        {
            throw new MakeReactionToOwnReviewException();
        }

        var existingReviewReaction = await _dbContext.ReviewReactions
            .FirstOrDefaultAsync(reviewReaction => reviewReaction.ReactorId == reactorId && reviewReaction.WriterId == writerId && reviewReaction.ProductSku == productSku, cancellationToken);

        if (existingReviewReaction != null)
        {
            existingReviewReaction.Type = type;
        }
        else
        {
            var reviewReaction = new ReviewReaction
            {
                ReactorId = reactorId,
                WriterId = writerId,
                ProductSku = productSku,
                Type = type
            };
            
            _dbContext.ReviewReactions.Add(reviewReaction);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}