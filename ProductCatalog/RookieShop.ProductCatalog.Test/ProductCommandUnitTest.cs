using MassTransit.Mediator;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Events;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Test.Utilities;

namespace RookieShop.ProductCatalog.Test;

public class ProductCommandUnitTest
{
    [Fact]
    public async Task Should_CreateProduct_FailedWithConflictingSku()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var createProduct = new CreateProduct
        {
            Sku = "SKYWATCH12DOB",
            Name = "Test Name",
            Description = "Test Description",
            Price = 1000,
            CategoryId = 1,
            PrimaryImageId = Guid.NewGuid(),
            SupportingImageIds = new HashSet<Guid>(),
            IsFeatured = true
        };
        
        // Act
        var createProductAction = async () => await scopedMediator.Send(createProduct);
        
        // Assert
        await Assert.ThrowsAsync<ProductAlreadyExistsException>(createProductAction);
    }

    [Fact]
    public async Task Should_CreateProduct_FailedWithNotFoundCategory()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        
        var createProduct = new CreateProduct
        {
            Sku = "TestSku",
            Name = "Test Name",
            Description = "Test Description",
            Price = 1000,
            CategoryId = 3,
            PrimaryImageId = Guid.NewGuid(),
            SupportingImageIds = new HashSet<Guid>(),
            IsFeatured = true
        };
        
        // Act
        var createProductAction = async () => await scopedMediator.Send(createProduct);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(createProductAction);
    }
    
    [Fact]
    public async Task Should_CreateProduct_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();

        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        
        var createProduct = new CreateProduct
        {
            Sku = "TestSku",
            Name = "Test Name",
            Description = "Test Description",
            Price = 1000,
            CategoryId = 2,
            PrimaryImageId = Guid.NewGuid(),
            SupportingImageIds = new HashSet<Guid>(),
            IsFeatured = true
        };

        try
        {
            await harness.Start();
            
            // Act
            await scopedMediator.Send(createProduct);

            // Assert
            var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();

            var product = await context.Products.FirstOrDefaultAsync(product => product.Sku == "TestSku");

            Assert.NotNull(product);
            
            Assert.True(await harness.Published.Any<ProductCreatedOrUpdated>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Should_UpdateProduct_FailedWithNotFoundSku()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var updateProduct = new UpdateProduct
        {
            Sku = "TestSku",
            Name = "Test Name",
            Description = "Test Description",
            Price = 1000,
            CategoryId = 1,
            PrimaryImageId = Guid.NewGuid(),
            SupportingImageIds = new HashSet<Guid>(),
            IsFeatured = true
        };
        
        // Act
        var updateProductAction = async () => await scopedMediator.Send(updateProduct);
        
        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(updateProductAction);
    }
    
    [Fact]
    public async Task Should_UpdateProduct_FailedWithNotFoundCategory()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var updateProduct = new UpdateProduct
        {
            Sku = "ASKAR160APO",
            Name = "Test Name",
            Description = "Test Description",
            Price = 1000,
            CategoryId = 3,
            PrimaryImageId = Guid.NewGuid(),
            SupportingImageIds = new HashSet<Guid>(),
            IsFeatured = true
        };
        
        // Act
        var updateProductAction = async () => await scopedMediator.Send(updateProduct);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(updateProductAction);
    }
    
    [Fact]
    public async Task Should_UpdateProduct_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        
        var updateProduct = new UpdateProduct
        {
            Sku = "ASKAR160APO",
            Name = "Test Name",
            Description = "Test Description",
            Price = 1000,
            CategoryId = 1,
            PrimaryImageId = Guid.NewGuid(),
            SupportingImageIds = new HashSet<Guid>(),
            IsFeatured = true
        };

        try
        {
            await harness.Start();
            
            // Act
            await scopedMediator.Send(updateProduct);
        
            // Assert
            var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
        
            var product = await context.Products.Include(product => product.Category)
                .FirstOrDefaultAsync(product => product.Sku == "ASKAR160APO");
        
            Assert.NotNull(product);
            Assert.Equal("Test Name", product.Name);
            Assert.Equal("Test Description", product.Description);
            Assert.Equal(1000, product.Price);
            Assert.Equal(1, product.Category.Id);
            Assert.True(product.IsFeatured);
            
            Assert.True(await harness.Published.Any<ProductCreatedOrUpdated>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Should_DeleteProduct_FailedWithNotFoundSku()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var deleteProduct = new DeleteProduct
        {
            Sku = "TestSku",
        };
        
        // Act
        var deleteProductAction = async () => await scopedMediator.Send(deleteProduct);
        
        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(deleteProductAction);
    }

    [Fact]
    public async Task Should_DeleteProduct_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        
        var deleteProduct = new DeleteProduct
        {
            Sku = "ASKAR160APO",
        };
        
        // Act
        await scopedMediator.Send(deleteProduct);
        
        // Assert
        var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
        
        var product = await context.Products.FirstOrDefaultAsync(product => product.Sku == "ASKAR160APO");
        
        Assert.Null(product);
    }
}