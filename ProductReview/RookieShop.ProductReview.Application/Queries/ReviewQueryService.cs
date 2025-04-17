using Microsoft.EntityFrameworkCore;
using RookieShop.ProductReview.Application.Abstractions;
using RookieShop.ProductReview.Application.Entities;
using RookieShop.ProductReview.Application.Models;

namespace RookieShop.ProductReview.Application.Queries;

public class ReviewQueryService
{
    private readonly ProductReviewDbContext _dbContext;

    public ReviewQueryService(ProductReviewDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Pagination<ReviewDto>> GetReviewsByProductSku(string productSku, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Reviews
            .OrderByDescending(review => review.CreatedDate)
            .Where(review => review.Id.ProductSku == productSku)
            .Select(review => new ReviewDto
            {
                WriterId = review.Id.WriterId,
                ProductSku = review.Id.ProductSku,
                Score = review.Score,
                Comment = review.Comment,
                CreatedDate = review.CreatedDate,
                NumberOfLikes = review.Reactions.Count(reaction => reaction.Type == ReactionType.Like),
                NumberOfDislikes = review.Reactions.Count(reaction => reaction.Type == ReactionType.Dislike)
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