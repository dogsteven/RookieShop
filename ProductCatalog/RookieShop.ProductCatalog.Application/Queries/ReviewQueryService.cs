using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.ProductCatalog.Application.Queries;

public class ReviewQueryService
{
    private readonly ProductCatalogDbContext _dbContext;

    public ReviewQueryService(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<Pagination<ReviewDto>> GetReviewsByProductSkuAsync(string productSku, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Reviews
            .Where(review => review.ProductSku == productSku)
            .OrderByDescending(review => review.CreatedDate)
            .Select(review => new ReviewDto
            {
                WriterId = review.WriterId,
                ProductSku = review.ProductSku,
                WriterName = review.WriterName,
                Score = review.Score,
                Comment = review.Comment,
                CreatedDate = review.CreatedDate,
                NumberOfLikes = review.Reactions.Count(reviewReaction => reviewReaction.Type == ReviewReactionType.Like),
                NumberOfDislikes = review.Reactions.Count(reviewReaction => reviewReaction.Type == ReviewReactionType.Dislike)
            })
            .AsNoTracking();

        var reviewDtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var count = await query.LongCountAsync(cancellationToken);

        return new Pagination<ReviewDto>
        {
            Count = count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = reviewDtos
        };
    }
}