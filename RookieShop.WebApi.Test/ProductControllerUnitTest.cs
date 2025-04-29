using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Application.Models;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.Shared.Models;
using RookieShop.WebApi.ProductCatalog.Controllers;
using RookieShop.WebApi.Test.Utilities;
using IScopedMediator = MassTransit.Mediator.IScopedMediator;

namespace RookieShop.WebApi.Test;

public class ProductControllerUnitTest
{
    [Fact]
    public async Task Should_GetProductsBySkuAsync_FailedWithNotFoundProduct()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockProductQueryService = scope.ServiceProvider.GetRequiredService<Mock<ProductQueryService>>();

        var sku = "ABC123";

        mockProductQueryService.Setup(productQueryService => productQueryService.GetProductBySkuAsync(sku, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProductNotFoundException(sku));
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();
        
        // Act
        var getProductBySkuAsyncAction = async () => await productController.GetProductBySkuAsync(sku, default);
        
        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(getProductBySkuAsyncAction);
    }

    [Fact]
    public async Task Should_GetProductsBySkuAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockProductQueryService = scope.ServiceProvider.GetRequiredService<Mock<ProductQueryService>>();

        var sku = "ABC123";
        var primaryImageId = Guid.NewGuid();

        mockProductQueryService.Setup(productQueryService => productQueryService.GetProductBySkuAsync(sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto
            {
                Sku = sku,
                Name = "Test Name",
                Description = "Test Description",
                Price = 1000,
                CategoryId = 1,
                CategoryName = "Test Category Name",
                PrimaryImageId = primaryImageId,
                SupportingImageIds = [],
                CreatedDate = DateTime.Today,
                UpdatedDate = DateTime.Today,
                IsFeatured = true,
            });
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();
        
        // Act
        var actionResult = await productController.GetProductBySkuAsync(sku, default);
        
        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);
        
        var okObjectResult = (OkObjectResult)actionResult.Result;
        
        Assert.IsType<ProductDto>(okObjectResult.Value);
        
        var productDto = (ProductDto)okObjectResult.Value;
        
        Assert.Equal(sku, productDto.Sku);
        Assert.Equal("Test Name", productDto.Name);
        Assert.Equal("Test Description", productDto.Description);
        Assert.Equal(1000, productDto.Price);
        Assert.Equal(1, productDto.CategoryId);
        Assert.Equal("Test Category Name", productDto.CategoryName);
        Assert.Equal(primaryImageId, productDto.PrimaryImageId);
        Assert.True(productDto.IsFeatured);
    }

    [Fact]
    public async Task Test_GetProductsAsync()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockProductQueryService = scope.ServiceProvider.GetRequiredService<Mock<ProductQueryService>>();

        mockProductQueryService.Setup(productQueryService => productQueryService.GetProductsAsync(1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<ProductDto>
            {
                Count = 3,
                PageNumber = 1,
                PageSize = 100,
                Items =
                [
                    new ProductDto
                    {
                        Sku = "ABC123"
                    },
                    new ProductDto
                    {
                        Sku = "BCA231"
                    },
                    new ProductDto
                    {
                        Sku = "ABC321"
                    }
                ]
            });
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();
        
        // Act
        var actionResult = await productController.GetProductsAsync(1, 100, default);
        
        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);
        
        var okObjectResult = (OkObjectResult)actionResult.Result;
        
        Assert.IsType<Pagination<ProductDto>>(okObjectResult.Value);
        
        var pagination = (Pagination<ProductDto>)okObjectResult.Value;
        
        Assert.Equal(3, pagination.Count);
        Assert.Equal(1, pagination.PageNumber);
        Assert.Equal(100, pagination.PageSize);
        Assert.Equal(["ABC123", "BCA231", "ABC321"], pagination.Items.Select(productDto => productDto.Sku));
    }

    [Fact]
    public async Task Test_GetFeaturedProductsAsync()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockProductQueryService = scope.ServiceProvider.GetRequiredService<Mock<ProductQueryService>>();

        mockProductQueryService.Setup(productQueryService => productQueryService.GetFeaturedProductsAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new ProductDto
                {
                    Sku = "ABC123",
                    IsFeatured = true,
                },
                new ProductDto
                {
                    Sku = "BCA231",
                    IsFeatured = true,
                },
                new ProductDto
                {
                    Sku = "ABC321",
                    IsFeatured = true,
                }
            ]);
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();
        
        // Act
        var actionResult = await productController.GetFeaturedProductsAsync(20, default);
        
        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);
        
        var okObjectResult = (OkObjectResult)actionResult.Result;
        
        Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okObjectResult.Value);
        
        var productDtos = ((IEnumerable<ProductDto>)okObjectResult.Value!).ToList();
        
        Assert.Equal(3, productDtos.Count);
        Assert.Equal(["ABC123", "BCA231", "ABC321"], productDtos.Select(productDto => productDto.Sku));
        Assert.All(productDtos, productDto => Assert.True(productDto.IsFeatured));
    }

    [Fact]
    public async Task Test_GetProductsByCategoryAsync()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockProductQueryService = scope.ServiceProvider.GetRequiredService<Mock<ProductQueryService>>();

        mockProductQueryService.Setup(productQueryService => productQueryService.GetProductsByCategoryAsync(1, 1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<ProductDto>
            {
                Count = 3,
                PageNumber = 1,
                PageSize = 100,
                Items =
                [
                    new ProductDto
                    {
                        Sku = "ABC123",
                        CategoryId = 1
                    },
                    new ProductDto
                    {
                        Sku = "BCA231",
                        CategoryId = 1
                    },
                    new ProductDto
                    {
                        Sku = "ABC321",
                        CategoryId = 1
                    }
                ]
            });
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();
        
        // Act
        var actionResult = await productController.GetProductsByCategoryAsync(1, 1, 100, default);
        
        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);
        
        var okObjectResult = (OkObjectResult)actionResult.Result;
        
        Assert.IsType<Pagination<ProductDto>>(okObjectResult.Value);
        
        var pagination = (Pagination<ProductDto>)okObjectResult.Value;
        
        Assert.Equal(3, pagination.Count);
        Assert.Equal(1, pagination.PageNumber);
        Assert.Equal(100, pagination.PageSize);
        Assert.Equal(["ABC123", "BCA231", "ABC321"], pagination.Items.Select(productDto => productDto.Sku));
        Assert.All(pagination.Items, productDto => Assert.Equal(1, productDto.CategoryId));
    }

    [Fact]
    public async Task Should_CreateProductAsync_FailedWithTakenSku()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<CreateProduct>(command => command.Sku == "ABC123"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProductSkuHasAlreadyBeenTakenException("ABC123"));
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();

        var createProductBody = new ProductsController.CreateProductBody
        {
            Sku = "ABC123",
            Name = "ABC123",
        };
        
        // Act
        var createProductAsyncAction = async () => await productController.CreateProductAsync(createProductBody, default);
        
        // Assert
        await Assert.ThrowsAsync<ProductSkuHasAlreadyBeenTakenException>(createProductAsyncAction);
    }

    [Fact]
    public async Task Should_CreateProductAsync_FailedWithNotFoundCategory()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<CreateProduct>(command => command.CategoryId == 2), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CategoryNotFoundException(2));
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();

        var createProductBody = new ProductsController.CreateProductBody
        {
            Sku = "ABC123",
            CategoryId = 2
        };
        
        // Act
        var createProductAsyncAction = async () => await productController.CreateProductAsync(createProductBody, default);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(createProductAsyncAction);
    }

    [Fact]
    public async Task Should_CreateProductAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.IsAny<CreateProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();

        var createProductBody = new ProductsController.CreateProductBody
        {
            Sku = "ABC123"
        };
        
        // Act
        var actionResult = await productController.CreateProductAsync(createProductBody, default);
        
        // Assert
        Assert.IsType<CreatedResult>(actionResult);
    }

    [Fact]
    public async Task Should_UpdateProductAsync_FailedWithNotFoundProduct()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<UpdateProduct>(command => command.Sku == "ABC123"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProductNotFoundException("ABC123"));
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();

        var updateProductBody = new ProductsController.UpdateProductBody
        {

        };

        // Act
        var updateProductAsyncAction = async () => await productController.UpdateProductAsync("ABC123", updateProductBody, default);
        
        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(updateProductAsyncAction);
    }

    [Fact]
    public async Task Should_UpdateProductAsync_FailedWithNotFoundCategory()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<UpdateProduct>(command => command.CategoryId == 1), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CategoryNotFoundException(1));
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();

        var updateProductBody = new ProductsController.UpdateProductBody
        {
            CategoryId = 1
        };

        // Act
        var updateProductAsyncAction = async () => await productController.UpdateProductAsync("ABC123", updateProductBody, default);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(updateProductAsyncAction);
    }

    [Fact]
    public async Task Should_UpdateProductAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.IsAny<UpdateProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();

        var updateProductBody = new ProductsController.UpdateProductBody
        {
        };

        // Act
        var actionResult = await productController.UpdateProductAsync("ABC123", updateProductBody, default);
        
        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task Should_DeleteProductAsync_FailedWithNotFoundProduct()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<DeleteProduct>(command => command.Sku == "ABC123"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProductNotFoundException("ABC123"));
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();

        // Act
        var deleteProductAsyncAction = async () => await productController.DeleteProductAsync("ABC123", default);
        
        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(deleteProductAsyncAction);
    }

    [Fact]
    public async Task Should_DeleteProductAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.IsAny<DeleteProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var productController = scope.ServiceProvider.GetRequiredService<ProductsController>();

        // Act
        var actionResult = await productController.DeleteProductAsync("ABC123", default);
        
        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }
}