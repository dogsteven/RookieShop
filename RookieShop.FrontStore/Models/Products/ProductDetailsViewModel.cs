using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models.Products;

public class ProductDetailsViewModel
{
    public Product Product { get; set; } = null!;
    
    public Pagination<Review> ReviewPage { get; set; } = null!;
}