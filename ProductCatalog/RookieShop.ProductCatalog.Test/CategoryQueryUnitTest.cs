using MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.Test.Utilities;

namespace RookieShop.ProductCatalog.Test;

public class CategoryQueryUnitTest
{
    [Fact]
    public async Task Test_GetCategories()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var categoryQueryService = scope.ServiceProvider.GetRequiredService<CategoryQueryService>();
        
        // Act
        var categoryDtos = (await categoryQueryService.GetCategoriesAsync(default)).ToList();
        
        // Assert
        Assert.Equal(2, categoryDtos.Count);
    }
    
    [Theory]
    [InlineData(3)]
    [InlineData(17)]
    [InlineData(512)]
    [InlineData(97)]
    public async Task Should_GetCategoryById_FailedWithNotFoundId(int id)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var categoryQueryService = scope.ServiceProvider.GetRequiredService<CategoryQueryService>();
        
        // Act
        var getCategoryByIdAction = async () => await categoryQueryService.GetCategoryByIdAsync(id, default);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(getCategoryByIdAction);
    }
    
    [Theory]
    [InlineData(1, "Reflecting Telescopes")]
    [InlineData(2, "Refracting Telescopes")]
    public async Task Should_GetCategoryById_Success(int id, string expectedName)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var categoryQueryService = scope.ServiceProvider.GetRequiredService<CategoryQueryService>();
        
        // Act
        var categoryDto = await categoryQueryService.GetCategoryByIdAsync(id, default);
        
        // Assert
        Assert.Equal(id, categoryDto.Id);
        Assert.Equal(expectedName, categoryDto.Name);
    }
}