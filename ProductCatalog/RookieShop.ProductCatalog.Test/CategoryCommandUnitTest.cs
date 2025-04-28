using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Test.Utilities;

namespace RookieShop.ProductCatalog.Test;

public class CategoryCommandUnitTest
{
    [Fact]
    public async Task Should_CreateCategory_FailedWithTakenName()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var createCategory = new CreateCategory
        {
            Name = "Reflecting Telescopes",
            Description = "Reflecting Telescopes Description"
        };
        
        // Act
        var createCategoryAction = async () => await scopedMediator.Send(createCategory);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNameHasAlreadyBeenTakenException>(createCategoryAction);
    }

    [Fact]
    public async Task Should_CreateCategory_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        var client = scopedMediator.CreateRequestClient<CreateCategory>();

        var createCategory = new CreateCategory
        {
            Name = "Catadioptric Telescopes",
            Description = "Catadioptric Telescopes Description"
        };
        
        // Act
        var response = await client.GetResponse<CategoryCreatedResponse>(createCategory);
        
        // Assert
        var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
        
        var category = await context.Categories.FirstOrDefaultAsync(category => category.Id == response.Message.Id);
        
        Assert.NotNull(category);
    }

    [Fact]
    public async Task Should_UpdateCategory_FailedWithNotFoundId()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var updateCategory = new UpdateCategory
        {
            Id = 3,
            Name = "Catadioptric Telescopes",
            Description = "Catadioptric Telescopes Description"
        };
        
        // Act
        var updateCategoryAction = async () => await scopedMediator.Send(updateCategory);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(updateCategoryAction);
    }

    [Fact]
    public async Task Should_UpdateCategory_FailedWithTakenName()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var updateCategory = new UpdateCategory
        {
            Id = 2,
            Name = "Reflecting Telescopes",
            Description = "Reflecting Telescopes Description"
        };
        
        // Act
        var updateCategoryAction = async () => await scopedMediator.Send(updateCategory);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNameHasAlreadyBeenTakenException>(updateCategoryAction);
    }

    [Fact]
    public async Task Should_UpdateCategory_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var updateCategory = new UpdateCategory
        {
            Id = 2,
            Name = "Catadioptric Telescopes",
            Description = "Catadioptric Telescopes Description"
        };
        
        // Act
        await scopedMediator.Send(updateCategory);
        
        // Assert
        var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
        
        var category = await context.Categories.FirstOrDefaultAsync(category => category.Id == 2);
        
        Assert.NotNull(category);
        Assert.Equal("Catadioptric Telescopes", category.Name);
        Assert.Equal("Catadioptric Telescopes Description", category.Description);
    }

    [Fact]
    public async Task Should_DeleteCategory_FailedWithNotFoundId()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var deleteCategory = new DeleteCategory
        {
            Id = 3,
        };
        
        // Act
        var deleteCategoryAction = async () => await scopedMediator.Send(deleteCategory);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(deleteCategoryAction);
    }
    
    [Fact]
    public async Task Should_DeleteCategory_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var deleteCategory = new DeleteCategory
        {
            Id = 1,
        };
        
        // Act
        await scopedMediator.Send(deleteCategory);
        
        // Assert
        var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
        
        var category = await context.Categories.FirstOrDefaultAsync(category => category.Id == 1);
        
        Assert.Null(category);
    }
}