using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Application.Events;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.ProductCatalog.Application.Commands;

public class SubmitReview
{
    public Guid WriterId { get; set; }

    public string ProductSku { get; set; } = null!;

    public string WriterName { get; set; } = null!;
    
    public int Score { get; set; }

    public string Comment { get; set; } = null!;
}

public class SubmitReviewConsumer : IConsumer<SubmitReview>
{
    private readonly ProductCatalogDbContext _dbContext;
    private readonly IProfanityChecker _profanityChecker;
    private readonly IPublishEndpoint _publishEndpoint;

    public SubmitReviewConsumer(ProductCatalogDbContext dbContext, IProfanityChecker profanityChecker, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _profanityChecker = profanityChecker;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<SubmitReview> context)
    {
        var message = context.Message;
        var writerId = message.WriterId;
        var productSku = message.ProductSku;
        var writerName = message.WriterName;
        var score = message.Score;
        var comment = message.Comment;
        
        var cancellationToken = context.CancellationToken;
        
        var hasProfanity = await _profanityChecker.CheckProfanityAsync(comment, cancellationToken);

        if (hasProfanity)
        {
            throw new ProfaneCommentException();
        }
        
        var productExists = await _dbContext.Products
            .AnyAsync(product => product.Sku == productSku, cancellationToken);

        if (!productExists)
        {
            throw new ProductNotFoundException(productSku);
        }

        var rating = new Review
        {
            WriterId = writerId,
            ProductSku = productSku,
            WriterName = writerName,
            Score = score,
            Comment = comment,
            CreatedDate = DateTime.UtcNow
        };
        
        _dbContext.Reviews.Add(rating);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new ReviewSubmitted
        {
            ProductSku = productSku,
            Score = score
        }, cancellationToken);
    }
}