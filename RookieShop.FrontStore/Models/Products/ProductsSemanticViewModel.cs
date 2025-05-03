using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models.Products;

public class ProductsSemanticViewModel
{
    public string Semantic { get; set; } = null!;
    
    public Pagination<ProductDto> ProductPage { get; set; } = null!;
}