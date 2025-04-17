using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Abstractions;
using RookieShop.FrontStore.Models;

namespace RookieShop.FrontStore.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    
    public HomeController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var featuredProductsTask = _productService.GetFeaturedProductsAsync(4, cancellationToken);
        var productPageTask = _productService.GetProductsAsync(1, 8, cancellationToken);
        var categoriesTask = _categoryService.GetCategoriesAsync(cancellationToken);
        
        await Task.WhenAll(featuredProductsTask, productPageTask, categoriesTask);
        
        var featuredProducts = featuredProductsTask.Result;
        var productPage = productPageTask.Result;
        var categories = categoriesTask.Result;
        
        return View(new HomeViewModel
        {
            FeaturedProducts = featuredProducts,
            ProductPage = productPage,
            Categories = categories
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}