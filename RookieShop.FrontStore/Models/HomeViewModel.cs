using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models;

public class HomeViewModel
{
    public IEnumerable<ProductDto> FeaturedProducts { get; set; } = null!;
    public Pagination<ProductDto> ProductPage { get; set; } = null!;
    public IEnumerable<CategoryDto> Categories { get; set; } = null!;
}