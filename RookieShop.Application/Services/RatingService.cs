using Microsoft.EntityFrameworkCore;
using RookieShop.Application.Abstractions;
using RookieShop.Application.Entities;
using RookieShop.Application.Exceptions;
using RookieShop.Application.Models;

namespace RookieShop.Application.Services;

public class RatingService
{
    private readonly RookieShopDbContext _dbContext;
    private readonly IProfanityChecker _profanityChecker;
    private readonly IPurchaseChecker _purchaseChecker;

    public RatingService(RookieShopDbContext dbContext, IProfanityChecker profanityChecker,
        IPurchaseChecker purchaseChecker)
    {
        _dbContext = dbContext;
        _profanityChecker = profanityChecker;
        _purchaseChecker = purchaseChecker;
    }

    public async Task<Pagination<RatingDto>> GetRatingsBySkuAsync(string sku, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Ratings
            .Where(r => r.Sku == sku)
            .OrderByDescending(r => r.CreatedDate)
            .AsNoTracking()
            .Select(rating => new RatingDto
            {
                CustomerId = rating.CustomerId,
                Sku = rating.Sku,
                Score = rating.Score,
                Comment = rating.Comment,
                CreatedDate = rating.CreatedDate
            });
        
        var ratingDtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var count = await query.LongCountAsync(cancellationToken);

        return new Pagination<RatingDto>
        {
            Count = count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = ratingDtos
        };
    }

    public async Task WriteRatingAsync(Guid customerId, string sku, float score, string comment,
        CancellationToken cancellationToken)
    {
        var hasProfanity = await _profanityChecker.CheckProfanityAsync(comment, cancellationToken);

        if (hasProfanity)
        {
            throw new ProfanityDetectedException();
        }
        
        var productExists = await _dbContext.Products.AnyAsync(p => p.Sku == sku, cancellationToken);

        if (!productExists)
        {
            throw new ProductNotFoundException(sku);
        }
        
        var hasPurchased = await _purchaseChecker.CheckIfCustomerHasPurchasedProductAsync(customerId, sku, cancellationToken);

        if (!hasPurchased)
        {
            throw new CustomerHasNotPurchasedException();
        }

        var rating = new Rating
        {
            CustomerId = customerId,
            Sku = sku,
            Score = score,
            Comment = comment,
            CreatedDate = DateTime.UtcNow
        };
        
        _dbContext.Ratings.Add(rating);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}