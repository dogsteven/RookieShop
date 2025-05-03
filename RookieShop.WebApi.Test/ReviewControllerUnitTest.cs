using System.Security.Claims;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;
using RookieShop.WebApi.ProductCatalog.Controllers;
using RookieShop.WebApi.Test.Utilities;

namespace RookieShop.WebApi.Test;

public class ReviewControllerUnitTest
{
    [Fact]
    public async Task Test_GetReviewsByProductSkuAsync()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var mockReviewQueryService = scope.ServiceProvider.GetRequiredService<Mock<ReviewQueryService>>();

        List<Guid> writerIds = [Guid.NewGuid(), Guid.NewGuid()];
        
        mockReviewQueryService.Setup(reviewQueryService => reviewQueryService.GetReviewsByProductSkuAsync("ABC123", 1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<ReviewDto>
            {
                Count = 2,
                PageNumber = 1,
                PageSize = 100,
                Items = [
                    new ReviewDto
                    {
                        WriterId = writerIds[0],
                        ProductSku = "ABC123",
                        WriterName = "OpenGL",
                        Score = 5,
                        Comment = "Some comment 1",
                        CreatedDate = DateTime.Today,
                        NumberOfLikes = 2,
                        NumberOfDislikes = 1
                    },
                    new ReviewDto
                    {
                        WriterId = writerIds[1],
                        ProductSku = "ABC123",
                        WriterName = "Vulkan",
                        Score = 4,
                        Comment = "Some comment 2",
                        CreatedDate = DateTime.Today.AddDays(-2),
                        NumberOfLikes = 10,
                        NumberOfDislikes = 1
                    },
                ]
            });
        
        var reviewController = scope.ServiceProvider.GetRequiredService<ReviewsController>();
        
        // Act
        var actionResult = await reviewController.GetReviewsByProductSkuAsync("ABC123", 1, 100);
        
        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);
        
        var okObjectResult = (OkObjectResult)actionResult.Result;
        
        Assert.IsType<Pagination<ReviewDto>>(okObjectResult.Value);
        
        var pagination = (Pagination<ReviewDto>)okObjectResult.Value;
        
        Assert.Equal(2, pagination.Count);
        Assert.Equal(1, pagination.PageNumber);
        Assert.Equal(100, pagination.PageSize);
        Assert.Equal(writerIds, pagination.Items.Select(reviewDto => reviewDto.WriterId));
    }

    [Fact]
    public async Task Should_SubmitReviewAsync_FailedWithNotFoundProduct()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var userId = Guid.NewGuid();
        
        var claimsIdentity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "John Doe"),
        ], "TestAuthType");

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        var mockHttpContext = new Mock<HttpContext>();

        mockHttpContext.Setup(httpContext => httpContext.User).Returns(claimsPrincipal);

        var controllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };

        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<SubmitReview>(command => command.ProductSku == "ABC123"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProductNotFoundException("ABC123"));

        var reviewController = scope.ServiceProvider.GetRequiredService<ReviewsController>();
        
        reviewController.ControllerContext = controllerContext;

        var submitReviewBody = new ReviewsController.SubmitReviewBody
        {

        };
        
        // Act
        var submitReviewAsyncAction = async () => await reviewController.SubmitReviewAsync("ABC123", submitReviewBody, default);
        
        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(submitReviewAsyncAction);
    }

    [Fact]
    public async Task Should_SubmitReviewAsync_FailedWithHasAlreadyWrittenReview()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var userId = Guid.NewGuid();
        
        var claimsIdentity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "John Doe"),
        ], "TestAuthType");

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        var mockHttpContext = new Mock<HttpContext>();

        mockHttpContext.Setup(httpContext => httpContext.User).Returns(claimsPrincipal);

        var controllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };
        
        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<SubmitReview>(command => command.WriterId == userId && command.ProductSku == "ABC123"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CustomerHasAlreadyWrittenReviewException(userId, "ABC123"));
        
        var reviewController = scope.ServiceProvider.GetRequiredService<ReviewsController>();
        
        reviewController.ControllerContext = controllerContext;

        var submitReviewBody = new ReviewsController.SubmitReviewBody
        {
            
        };
        
        // Act
        var submitReviewAsyncAction = async () => await reviewController.SubmitReviewAsync("ABC123", submitReviewBody, default);
        
        // Assert
        await Assert.ThrowsAsync<CustomerHasAlreadyWrittenReviewException>(submitReviewAsyncAction);
    }

    [Fact]
    public async Task Should_SubmitReviewAsync_FailedWithProfanityDetected()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var userId = Guid.NewGuid();
        
        var claimsIdentity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "John Doe"),
        ], "TestAuthType");

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        var mockHttpContext = new Mock<HttpContext>();

        mockHttpContext.Setup(httpContext => httpContext.User).Returns(claimsPrincipal);

        var controllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };
        
        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.Is<SubmitReview>(command => command.Comment == "This is fucking bad"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProfaneCommentException());
        
        var reviewController = scope.ServiceProvider.GetRequiredService<ReviewsController>();
        
        reviewController.ControllerContext = controllerContext;

        var submitReviewBody = new ReviewsController.SubmitReviewBody
        {
            Comment = "This is fucking bad"
        };
        
        // Act
        var submitReviewAsyncAction = async () => await reviewController.SubmitReviewAsync("ABC123", submitReviewBody, default);
        
        // Assert
        await Assert.ThrowsAsync<ProfaneCommentException>(submitReviewAsyncAction);
    }

    [Fact]
    public async Task Should_SubmitReviewAsync_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var userId = Guid.NewGuid();
        
        var claimsIdentity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "John Doe"),
        ], "TestAuthType");

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        var mockHttpContext = new Mock<HttpContext>();

        mockHttpContext.Setup(httpContext => httpContext.User).Returns(claimsPrincipal);

        var controllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };
        
        var mockScopedMediator = scope.ServiceProvider.GetRequiredService<Mock<IScopedMediator>>();

        mockScopedMediator.Setup(scopedMediator => scopedMediator.Send(It.IsAny<SubmitReview>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var reviewController = scope.ServiceProvider.GetRequiredService<ReviewsController>();
        
        reviewController.ControllerContext = controllerContext;

        var submitReviewBody = new ReviewsController.SubmitReviewBody
        {
            
        };
        
        // Act
        var actionResult = await reviewController.SubmitReviewAsync("ABC123", submitReviewBody, default);
        
        // Assert
        Assert.IsType<CreatedResult>(actionResult);
    }
}