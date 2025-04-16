using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Models;

public class HomeViewModel
{
    public IEnumerable<ProductDto> FeaturedProducts { get; set; } = null!;
    public Pagination<ProductDto> ProductPage { get; set; } = null!;
    public IEnumerable<CategoryDto> Categories { get; set; } = null!;
}