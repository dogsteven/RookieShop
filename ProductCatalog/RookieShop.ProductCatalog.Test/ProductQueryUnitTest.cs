using Microsoft.Extensions.DependencyInjection;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.Test.Utilities;

namespace RookieShop.ProductCatalog.Test;

public class ProductQueryUnitTest
{
    [Theory]
    [InlineData("TestSku")]
    [InlineData("YesYesYes")]
    [InlineData("NoNoNo")]
    public async Task Should_GetProductBySku_FailedWithNotFoundSku(string sku)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var productQueryService = scope.ServiceProvider.GetRequiredService<ProductQueryService>();
        
        // Act
        var getProductBySkuAction = async () => await productQueryService.GetProductBySkuAsync(sku, default);
        
        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(getProductBySkuAction);
    }
    
    [Theory]
    [InlineData("CELE114LCM","Celestron 114LCM Newtonian", 1)]
    [InlineData("SKYWATCH12DOB","Sky-Watcher 12\" Dobsonian", 1)]
    [InlineData("BRESSARCT60","Bresser Arcturus 60/700", 2)]
    [InlineData("ASKAR160APO","Askar 203 APO Triplet", 2)]
    public async Task Should_GetProductBySku_Success(string sku, string expectedName, int expectedCategoryId)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var productQueryService = scope.ServiceProvider.GetRequiredService<ProductQueryService>();
        
        // Act
        var productDto = await productQueryService.GetProductBySkuAsync(sku, default);
        
        // Assert
        Assert.Equal(sku, productDto.Sku);
        Assert.Equal(expectedName, productDto.Name);
        Assert.Equal(expectedCategoryId, productDto.CategoryId);
    }
    
    [Theory]
    [InlineData(1, 3, new [] { "BRESSARCT60", "SKYWATCH12DOB", "ASKAR160APO" })]
    [InlineData(2, 2, new [] { "ASKAR160APO", "CELE114LCM" })]
    [InlineData(2, 3, new [] { "CELE114LCM" })]
    [InlineData(1, 4, new [] { "BRESSARCT60", "SKYWATCH12DOB", "ASKAR160APO", "CELE114LCM" })]
    [InlineData(1, 6, new [] { "BRESSARCT60", "SKYWATCH12DOB", "ASKAR160APO", "CELE114LCM" })]
    [InlineData(2, 4, new string[] {})]
    [InlineData(2, 7, new string[] {})]
    public async Task Test_GetProducts(int pageNumber, int pageSize, IEnumerable<string> expectedSkus)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var productQueryService = scope.ServiceProvider.GetRequiredService<ProductQueryService>();
        
        // Act
        var pagination = await productQueryService.GetProductsAsync(pageNumber, pageSize, default);
        
        // Assert
        Assert.Equal(4, pagination.Count);
        Assert.Equal(pageNumber, pagination.PageNumber);
        Assert.Equal(pageSize, pagination.PageSize);
        Assert.Equal(expectedSkus, pagination.Items.Select(productDto => productDto.Sku));
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    public async Task Test_GetFeaturedProducts(int maxCount, int expectedCount)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var productQueryService = scope.ServiceProvider.GetRequiredService<ProductQueryService>();
        
        // Act
        var productDtos = (await productQueryService.GetFeaturedProductsAsync(maxCount, default)).ToList();
        
        // Assert
        Assert.Equal(expectedCount, productDtos.Count);
        
        Assert.All(productDtos, productDto => Assert.True(productDto.IsFeatured));
    }

    [Theory]
    [InlineData(3, 1, 3)]
    [InlineData(85, 6, 5)]
    [InlineData(70, 2, 6)]
    [InlineData(2834, 8, 6)]
    [InlineData(93, 10, 423)]
    public async Task Test_GetProductsByCategory_WithNonExistingCategory(int categoryId, int pageNumber, int pageSize)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var productQueryService = scope.ServiceProvider.GetRequiredService<ProductQueryService>();
        
        // Act
        var pagination = await productQueryService.GetProductsByCategoryAsync(categoryId, pageNumber, pageSize, default);
        
        // Assert
        Assert.Equal(0, pagination.Count);
    }

    [Theory]
    [InlineData(1, 1, 3, 2)]
    [InlineData(1, 2, 4, 2)]
    [InlineData(2, 1, 2, 2)]
    [InlineData(2, 2, 5, 2)]
    public async Task Test_GetProductsByCategory_WithExistingCategory(int categoryId, int pageNumber, int pageSize, int expectedCount)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var productQueryService = scope.ServiceProvider.GetRequiredService<ProductQueryService>();
        
        // Act
        var pagination = await productQueryService.GetProductsByCategoryAsync(categoryId, pageNumber, pageSize, default);
        
        // Assert
        Assert.Equal(expectedCount, pagination.Count);
    }
}