using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductReview.Application.Abstractions;
using RookieShop.ProductReview.Application.Entities;

namespace RookieShop.ProductReview.Application.Commands;

public class MakeReaction
{
    public Guid ReactorId { get; set; }
    
    public Guid WriterId { get; set; }

    public string ProductSku { get; set; } = null!;
    
    public ReactionType ReactionType { get; set; }
}

public class MakeReactionConsumer : IConsumer<MakeReaction>
{
    private readonly ProductReviewDbContext _dbContext;

    public MakeReactionConsumer(ProductReviewDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<MakeReaction> context)
    {
        var message = context.Message;
        var reactorId = message.ReactorId;
        var writerId = message.WriterId;
        var productSku = message.ProductSku;
        var reactionType = message.ReactionType;
        
        var cancellationToken = context.CancellationToken;

        var existingReaction = await _dbContext.Reactions
            .FirstOrDefaultAsync(reaction => reaction.ReactorId == reactorId && reaction.WriterId == writerId && reaction.ProductSku == productSku, cancellationToken);

        if (existingReaction != null)
        {
            existingReaction.Type = reactionType;
        }
        else
        {
            var reaction = new Reaction
            {
                ReactorId = reactorId,
                WriterId = writerId,
                ProductSku = productSku,
                Type = reactionType
            };
            
            _dbContext.Reactions.Add(reaction);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}