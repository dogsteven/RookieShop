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

    public async Task<IActionResult> ProductCatalog(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var productPage = await _productService.GetProductsAsync(int.Max(pageNumber ?? 1, 1), int.Max(pageSize ?? 12, 8), cancellationToken);

        return View(new ProductCatalogViewModel
        {
            ProductPage = productPage
        });
    }

    public async Task<IActionResult> ProductsByCategory(int id, int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var categoryTask = _categoryService.GetCategoryByIdAsync(id, cancellationToken);
        var productPageTask = _productService.GetProductsByCategoryIdAsync(id, int.Max(pageNumber ?? 1, 1), int.Max(pageSize ?? 12, 8), cancellationToken);
        
        await Task.WhenAll(categoryTask, productPageTask);
        
        var category = categoryTask.Result;
        var productPage = productPageTask.Result;

        return View(new ProductsByCategoryViewModel
        {
            Category = category,
            ProductPage = productPage
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}