using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Models.ProductCatalog;

public class ProductDetailsViewModel
{
    public ProductDto Product { get; set; } = null!;
    public Pagination<ReviewDto> ReviewPage { get; set; } = null!;
}