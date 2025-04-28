using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Application.Models;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.WebApi.Controllers;
using RookieShop.WebApi.Test.Utilities;
using Xunit.Abstractions;

namespace RookieShop.WebApi.Test;

public class CategoryControllerUnitTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CategoryControllerUnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task Should_GetCategoryByIdAsync_FailedWithNotFoundCategory()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockCategoryQueryService = scope.ServiceProvider.GetRequiredService<Mock<CategoryQueryService>>();

        var id = 1;

        mockCategoryQueryService.Setup(categoryQueryService => categoryQueryService.GetCategoryByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CategoryNotFoundException(id));
        
        var categoryController = scope.ServiceProvider.GetRequiredService<CategoryController>();
        
        // Act
        var getCategoryByIdAction = async () => await categoryController.GetCategoryByIdAsync(id, default);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(getCategoryByIdAction);
    }
    
    [Fact]
    public async Task Should_GetCategoryByIdAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var mockCategoryQueryService = scope.ServiceProvider.GetRequiredService<Mock<CategoryQueryService>>();

        mockCategoryQueryService.Setup(categoryQueryService => categoryQueryService.GetCategoryByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CategoryDto
            {
                Id = 1,
                Name = "Test Name",
                Description = "Test Description"
            });
        
        var categoryController = scope.ServiceProvider.GetRequiredService<CategoryController>();
        
        // Act
        var actionResult = await categoryController.GetCategoryByIdAsync(1, default);
        
        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);
        
        var okObjectResult = (OkObjectResult)actionResult.Result;
        
        Assert.IsType<CategoryDto>(okObjectResult.Value);
        
        var categoryDto = (CategoryDto)okObjectResult.Value;
        
        Assert.Equal(1, categoryDto.Id);
        Assert.Equal("Test Name", categoryDto.Name);
        Assert.Equal("Test Description", categoryDto.Description);
    }

    [Fact]
    public async Task Test_GetCategoriesAsync()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var mockCategoryQueryService = scope.ServiceProvider.GetRequiredService<Mock<CategoryQueryService>>();

        mockCategoryQueryService.Setup(categoryQueryService => categoryQueryService.GetCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new CategoryDto
                {
                    Id = 1,
                    Name = "Test Name 1",
                    Description = "Test Description 1"
                },
                new CategoryDto
                {
                    Id = 2,
                    Name = "Test Name 2",
                    Description = "Test Description 2"
                }
            ]);
        
        var categoryController = scope.ServiceProvider.GetRequiredService<CategoryController>();
        
        // Act
        var actionResult = await categoryController.GetCategoriesAsync(default);
        
        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);
        
        var okObjectResult = (OkObjectResult)actionResult.Result;
        
        Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(okObjectResult.Value);
        
        var categoryDtos = ((IEnumerable<CategoryDto>)okObjectResult.Value!).ToList();
        
        Assert.Equal(2, categoryDtos.Count);
        Assert.Equal(["Test Name 1", "Test Name 2"], categoryDtos.Select(categoryDto => categoryDto.Name));
        Assert.Equal(["Test Description 1", "Test Description 2"], categoryDtos.Select(categoryDto => categoryDto.Description));
    }

    [Fact]
    public async Task Should_CreateCategoryAsync_FailedWithConflictingCategory()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var mockRequestClient = new Mock<IRequestClient<CreateCategory>>();
        
        mockRequestClient.Setup(requestClient => requestClient.GetResponse<CategoryCreatedResponse>(It.Is<CreateCategory>(command => command.Name == "Test Name"), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ThrowsAsync(new RequestException("Request exception", new CategoryNameHasAlreadyBeenTakenException("Test Name")));
        
        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.CreateRequestClient<CreateCategory>(It.IsAny<RequestTimeout>()))
            .Returns(() => mockRequestClient.Object);
        
        var controller = scope.ServiceProvider.GetRequiredService<CategoryController>();

        var createCategoryBody = new CategoryController.CreateCategoryBody
        {
            Name = "Test Name",
            Description = "Test Description"
        };
        
        // Act
        var createCategoryAsyncAction = async () => await controller.CreateCategoryAsync(createCategoryBody, default);
        
        // Assert
        var exception = await Assert.ThrowsAsync<RequestException>(createCategoryAsyncAction);

        Assert.IsType<CategoryNameHasAlreadyBeenTakenException>(exception.InnerException);
    }

    [Fact]
    public async Task Should_CreateCategoryAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var mockResponse = new Mock<Response<CategoryCreatedResponse>>();

        mockResponse.Setup(response => response.Message)
            .Returns(() => new CategoryCreatedResponse { Id = 1 });
        
        var mockRequestClient = new Mock<IRequestClient<CreateCategory>>();
        
        mockRequestClient.Setup(requestClient => requestClient.GetResponse<CategoryCreatedResponse>(It.IsAny<CreateCategory>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ReturnsAsync(() => mockResponse.Object);
        
        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.CreateRequestClient<CreateCategory>(It.IsAny<RequestTimeout>()))
            .Returns(() => mockRequestClient.Object);
        
        var controller = scope.ServiceProvider.GetRequiredService<CategoryController>();
        
        var createCategoryBody = new CategoryController.CreateCategoryBody
        {
            Name = "Test Name",
            Description = "Test Description"
        };

        // Act
        var actionResult = await controller.CreateCategoryAsync(createCategoryBody, default);
        
        // Assert
        Assert.IsType<CreatedResult>(actionResult.Result);
    }

    [Fact]
    public async Task Should_UpdateCategoryAsync_FailedWithNotFoundCategory()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var mockedScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();
        
        mockedScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<UpdateCategory>(command => command.Id == 1), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CategoryNotFoundException(1));
        
        var controller = scope.ServiceProvider.GetRequiredService<CategoryController>();

        var updateCategoryBody = new CategoryController.UpdateCategoryBody
        {
            Name = "Test Name",
            Description = "Test Description"
        };
        
        // Act
        var updateCategoryAsyncAction = async () => await controller.UpdateCategoryAsync(1, updateCategoryBody, default);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(updateCategoryAsyncAction);
    }

    [Fact]
    public async Task Should_UpdateCategoryAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var mockedScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();
        
        mockedScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.IsAny<UpdateCategory>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var controller = scope.ServiceProvider.GetRequiredService<CategoryController>();

        var updateCategoryBody = new CategoryController.UpdateCategoryBody
        {
            Name = "Test Name",
            Description = "Test Description"
        };
        
        // Act
        var actionResult = await controller.UpdateCategoryAsync(1, updateCategoryBody, default);
        
        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task Should_DeleteCategoryAsync_FailedWithNotFoundCategory()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var mockedScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();
        
        mockedScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<DeleteCategory>(command => command.Id == 1), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CategoryNotFoundException(1));
        
        var controller = scope.ServiceProvider.GetRequiredService<CategoryController>();
        
        // Act
        var deleteCategoryAsyncAction = async () => await controller.DeleteCategoryAsync(1, default);
        
        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(deleteCategoryAsyncAction);
    }
    
    [Fact]
    public async Task Should_DeleteCategoryAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var mockedScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();
        
        mockedScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.IsAny<DeleteCategory>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var controller = scope.ServiceProvider.GetRequiredService<CategoryController>();
        
        // Act
        var actionResult = await controller.DeleteCategoryAsync(1, default);
        
        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }
}