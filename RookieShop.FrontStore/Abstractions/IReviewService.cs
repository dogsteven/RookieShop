using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Abstractions;

public interface IReviewService
{
    public Task<Pagination<RatingDto>> GetRatingsBySkuAsync(string sku, int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task WriteRatingAsync(string sku, float score, string comment, CancellationToken cancellationToken);
}