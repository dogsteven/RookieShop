using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Models;

public class ProductsByCategoryViewModel
{
    public CategoryDto Category { get; set; } = null!;
    public Pagination<ProductDto> ProductPage { get; set; } = null!;
}