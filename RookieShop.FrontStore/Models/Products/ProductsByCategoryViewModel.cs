using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models.Products;

public class ProductsByCategoryViewModel
{
    public CategoryDto Category { get; set; } = null!;
    public Pagination<ProductDto> ProductPage { get; set; } = null!;
}