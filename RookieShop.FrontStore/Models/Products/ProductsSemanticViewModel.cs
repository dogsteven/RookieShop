using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models.Products;

public class ProductsSemanticViewModel
{
    public string Semantic { get; set; } = null!;
    
    public Pagination<Product> ProductPage { get; set; } = null!;
}