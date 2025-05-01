using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;

public interface IReviewService
{
    public Task<Pagination<Review>> GetReviewsBySkuAsync(string sku, int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task SubmitReviewAsync(string sku, int score, string comment, CancellationToken cancellationToken);
    public Task MakeReactionAsync(Guid writerId, string sku, bool likeReaction, CancellationToken cancellationToken);
}