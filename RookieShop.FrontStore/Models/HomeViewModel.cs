using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models;

public class HomeViewModel
{
    public IEnumerable<Product> FeaturedProducts { get; set; } = null!;
    public Pagination<Product> ProductPage { get; set; } = null!;
    public IEnumerable<Category> Categories { get; set; } = null!;
}