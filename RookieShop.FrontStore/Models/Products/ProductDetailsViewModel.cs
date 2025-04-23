using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Models.Products;

public class ProductDetailsViewModel
{
    public Product Product { get; set; } = null!;
    
    public Pagination<Review> ReviewPage { get; set; } = null!;

    [FromForm(Name = "Score"), Required, Range(1, 5)]
    public int Score { get; set; }
    
    [FromForm(Name = "Comment"), Required, MinLength(1), MaxLength(250)]
    public string Comment { get; set; } = string.Empty;
}