using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models;

public class ProductsViewModel
{
    public Pagination<ProductDto> ProductPage { get; set; } = null!;
}