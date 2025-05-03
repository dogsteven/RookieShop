using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models.Products;

public class ProductDetailsViewModel
{
    public ProductDto Product { get; set; } = null!;
    
    public Pagination<ReviewDto> ReviewPage { get; set; } = null!;
}