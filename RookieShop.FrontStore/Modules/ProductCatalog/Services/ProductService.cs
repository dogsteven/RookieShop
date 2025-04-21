using System.Web;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;
using IProductService = RookieShop.FrontStore.Modules.ProductCatalog.Abstractions.IProductService;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly string _imageGalleryBasePath;

    public ProductService(IHttpClientFactory httpClientFactory, string imageGalleryBasePath)
    {
        _httpClient = httpClientFactory.CreateClient("RookieShop.WebApi");
        _imageGalleryBasePath = imageGalleryBasePath;
    }

    private class ProductDto
    {
        public string Sku { get; init; } = null!;
    
        public string Name { get; init; } = null!;
    
        public string Description { get; init; } = null!;
    
        public decimal Price { get; init; }
    
        public int CategoryId { get; init; }
    
        public string CategoryName { get; init; } = null!;
    
        public Guid PrimaryImageId { get; init; }
    
        public bool IsFeatured { get; init; }
    
        public DateTime CreatedDate { get; init; }
    
        public DateTime UpdatedDate { get; init; }

        public RatingDto Rating { get; init; } = null!;
    }
    
    private class RatingDto
    {
        public double Score { get; init; }
    
        public int OneCount { get; init; }
    
        public int TwoCount { get; init; }
    
        public int ThreeCount { get; init; }
    
        public int FourCount { get; init; }
    
        public int FiveCount { get; init; }
    }

    private Product Map(ProductDto product)
    {
        return new Product
        {
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.CategoryName,
            PrimaryImageUrl = $"{_imageGalleryBasePath}/api/ImageGallery/{product.PrimaryImageId}",
            IsFeatured = product.IsFeatured,
            CreatedDate = product.CreatedDate,
            UpdatedDate = product.UpdatedDate,
            Rating = new Rating
            {
                Score = product.Rating.Score,
                OneCount = product.Rating.OneCount,
                TwoCount = product.Rating.TwoCount,
                ThreeCount = product.Rating.ThreeCount,
                FourCount = product.Rating.FourCount,
                FiveCount = product.Rating.FiveCount
            }
        };
    }
    
    public async Task<Product> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/{sku}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var productDto = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(productDto);
        
        return Map(productDto);
    }

    public async Task<Pagination<Product>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageNumber"] = $"{pageNumber}";
        queries["pageSize"] = $"{pageSize}";
        
        var queryString = queries.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/all?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var pagination = await response.Content.ReadFromJsonAsync<Pagination<ProductDto>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(pagination);
        
        return new Pagination<Product>
        {
            Count = pagination.Count,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Items = pagination.Items.Select(Map)
        };
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int maxCount, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["maxCount"] = $"{maxCount}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/featured?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var productDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(productDtos);
        
        return productDtos.Select(Map);
    }

    public async Task<Pagination<Product>> GetProductsByCategoryIdAsync(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageSize"] = $"{pageSize}";
        queries["pageNumber"] = $"{pageNumber}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/by-category/{categoryId}?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var pagination = await response.Content.ReadFromJsonAsync<Pagination<ProductDto>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(pagination);
        
        return new Pagination<Product>
        {
            Count = pagination.Count,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Items = pagination.Items.Select(Map)
        };
    }
}