using MassTransit;
using RookieShop.ProductReview.Application.Abstractions;
using RookieShop.ProductReview.Application.Entities;
using RookieShop.ProductReview.Application.Exceptions;
using RookieShop.ProductReview.Contracts.Events;

namespace RookieShop.ProductReview.Application.Commands;

public class WriteReview
{
    public Guid WriterId { get; set; }

    public string ProductSku { get; set; } = null!;
    
    public int Score { get; set; }

    public string Comment { get; set; } = null!;
}

public class WriteReviewConsumer : IConsumer<WriteReview>
{
    private readonly ProductReviewDbContext _dbContext;
    private readonly IProfanityChecker _profanityChecker;

    public WriteReviewConsumer(ProductReviewDbContext dbContext, IProfanityChecker profanityChecker)
    {
        _dbContext = dbContext;
        _profanityChecker = profanityChecker;
    }
    
    public async Task Consume(ConsumeContext<WriteReview> context)
    {
        var message = context.Message;
        var writerId = message.WriterId;
        var productSku = message.ProductSku;
        var score = message.Score;
        var comment = message.Comment;
        
        var cancellationToken = context.CancellationToken;
        
        var hasProfanity = await _profanityChecker.CheckProfanityAsync(comment, cancellationToken);

        if (hasProfanity)
        {
            throw new ProfaneCommentException();
        }

        var rating = new Review
        {
            Id = new ReviewId(writerId, productSku),
            Score = score,
            Comment = comment,
            CreatedDate = DateTime.UtcNow
        };
        
        _dbContext.Reviews.Add(rating);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await context.Publish(new ReviewWrote
        {
            WriterId = writerId,
            ProductSku = productSku,
            Score = score
        }, cancellationToken);
    }
}