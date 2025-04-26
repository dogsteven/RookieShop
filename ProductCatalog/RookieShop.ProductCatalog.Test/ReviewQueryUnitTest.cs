using Microsoft.Extensions.DependencyInjection;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.Test.Utilities;

namespace RookieShop.ProductCatalog.Test;

public class ReviewQueryUnitTest
{
    [Theory]
    [InlineData("CELE114LCM", 1, 10, 2, new int[] { 3, 5 })]
    [InlineData("CELE114LCM", 1, 1, 2, new int[] { 3 })]
    [InlineData("SKYWATCH12DOB", 1, 5, 0, new int[] { })]
    [InlineData("ASKAR160APO", 1, 7, 1, new int[] { 4 })]
    [InlineData("NonExistingProductSku", 1, 10, 0, new int[] { })]
    public async Task Test_GetReviewsByProductSku(string productSku, int pageNumber, int pageSize, int expectedCount, IEnumerable<int> expectedScores)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();

        var reviewQueryService = scope.ServiceProvider.GetRequiredService<ReviewQueryService>();
        
        // Act
        var pagination = await reviewQueryService.GetReviewsByProductSku(productSku, pageNumber, pageSize, default);
        
        // Assert
        Assert.Equal(expectedCount, pagination.Count);
        Assert.Equal(pagination.PageNumber, pageNumber);
        Assert.Equal(pagination.PageSize, pageSize);
        Assert.Equal(expectedScores, pagination.Items.Select(review => review.Score));
    }
}