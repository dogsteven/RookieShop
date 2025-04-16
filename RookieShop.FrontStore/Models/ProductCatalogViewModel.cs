using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Models;

public class ProductCatalogViewModel
{
    public Pagination<ProductDto> ProductPage { get; set; } = null!;
}