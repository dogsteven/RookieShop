using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models;

public class ProductCatalogViewModel
{
    public Pagination<Product> ProductPage { get; set; } = null!;
}