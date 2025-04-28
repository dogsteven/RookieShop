using MassTransit.Mediator;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Events;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Test.Utilities;

namespace RookieShop.ProductCatalog.Test;

public class ReviewCommandUnitTest
{
    [Fact]
    public async Task Should_SubmitReview_FailedWithNotFoundProduct()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();
        
        var mockProfanityChecker = provider.GetRequiredService<Mock<IProfanityChecker>>();
        
        mockProfanityChecker.Setup(checker => checker.CheckProfanityAsync("This is fucking bad", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
            
        mockProfanityChecker.Setup(checker => checker.CheckProfanityAsync(It.Is<string>(text => text != "This is fucking bad"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var submitReview = new SubmitReview
        {
            WriterId = seeder.CustomerId,
            ProductSku = "NonExistingSku",
            WriterName = "Khoa",
            Score = 5,
            Comment = "This is fucking bad"
        };
        
        // Act
        var submitReviewAction = async () => await scopedMediator.Send(submitReview);
        
        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(submitReviewAction);
    }
    
    [Fact]
    public async Task Should_SubmitReview_FailedWithHasAlreadyWrittenReview()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        var mockProfanityChecker = provider.GetRequiredService<Mock<IProfanityChecker>>();
        
        mockProfanityChecker.Setup(checker => checker.CheckProfanityAsync("This is fucking bad", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
            
        mockProfanityChecker.Setup(checker => checker.CheckProfanityAsync(It.Is<string>(text => text != "This is fucking bad"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var submitReview = new SubmitReview
        {
            WriterId = seeder.CustomerId,
            ProductSku = "ASKAR160APO",
            WriterName = "Khoa",
            Score = 5,
            Comment = "This is fucking bad"
        };
        
        // Act
        var submitReviewAction = async () => await scopedMediator.Send(submitReview);
        
        // Assert
        await Assert.ThrowsAsync<CustomerHasAlreadyWrittenReviewException>(submitReviewAction);
    }
    
    [Fact]
    public async Task Should_SubmitReview_FailedWithProfanityDetected()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        var mockProfanityChecker = provider.GetRequiredService<Mock<IProfanityChecker>>();
        
        mockProfanityChecker.Setup(checker => checker.CheckProfanityAsync("This is fucking bad", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
            
        mockProfanityChecker.Setup(checker => checker.CheckProfanityAsync(It.Is<string>(text => text != "This is fucking bad"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var submitReview = new SubmitReview
        {
            WriterId = seeder.CustomerId,
            ProductSku = "BRESSARCT60",
            WriterName = "Khoa",
            Score = 5,
            Comment = "This is fucking bad"
        };
        
        // Act
        var submitReviewAction = async () => await scopedMediator.Send(submitReview);
        
        // Assert
        await Assert.ThrowsAsync<ProfaneCommentException>(submitReviewAction);
    }
    
    [Fact]
    public async Task Should_SubmitReview_Success()
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        var mockProfanityChecker = provider.GetRequiredService<Mock<IProfanityChecker>>();
        
        mockProfanityChecker.Setup(checker => checker.CheckProfanityAsync("This is fucking bad", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
            
        mockProfanityChecker.Setup(checker => checker.CheckProfanityAsync(It.Is<string>(text => text != "This is fucking bad"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        using var scope = provider.CreateScope();

        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var submitReview = new SubmitReview
        {
            WriterId = seeder.CustomerId,
            ProductSku = "BRESSARCT60",
            WriterName = "Khoa",
            Score = 5,
            Comment = "This is so good"
        };

        try
        {
            await harness.Start();
            
            // Act
            await scopedMediator.Send(submitReview);
        
            // Assert
            
            // Assert review has been persisted
            var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
            
            var review = await context.Reviews
                .FirstOrDefaultAsync(review => review.WriterId == seeder.CustomerId && review.ProductSku == submitReview.ProductSku);
            
            Assert.NotNull(review);
            
            // Assert ReviewSubmitted event has been published
            Assert.True(await harness.Published.Any<ReviewSubmitted>());
        }
        finally
        {
            await harness.Stop();
        }
    }
    
    [Theory]
    [InlineData("ASKAR160APO", 1, 2.5)]
    [InlineData("ASKAR160APO", 2, 3)]
    [InlineData("ASKAR160APO", 3, 3.5)]
    [InlineData("ASKAR160APO", 4, 4)]
    [InlineData("ASKAR160APO", 5, 4.5)]
    [InlineData("BRESSARCT60", 1, 1)]
    [InlineData("BRESSARCT60", 2, 2)]
    [InlineData("BRESSARCT60", 3, 3)]
    [InlineData("BRESSARCT60", 4, 4)]
    [InlineData("CELE114LCM", 1, 3)]
    [InlineData("CELE114LCM", 4, 4)]
    public async Task Test_ApplyScore(string productSku, int score, double expectedAppliedScore)
    {
        // Arrange
        var services = new ProductCatalogServiceCollection();
        
        var provider = services.BuildServiceProvider();

        var seeder = new ProductCatalogDatabaseSeeder(provider);

        await seeder.SeedAsync();
        
        using var scope = provider.CreateScope();

        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        try
        {
            await harness.Start();
            
            // Act
            await harness.Bus.Publish(new ReviewSubmitted
            {
                ProductSku = productSku,
                Score = score,
            });
        
            // Assert
            
            // Assert ReviewSubmitted event has been consumed by the right consumer
            var applyScoreConsumer = harness.GetConsumerHarness<ApplyScoreConsumer>();
            
            Assert.True(await applyScoreConsumer.Consumed.Any<ReviewSubmitted>());
            
            // Assert score has been applied 
            var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
            
            var productRating = await context.ProductRatings.FirstAsync(productRating => productRating.ProductSku == productSku);
            
            Assert.Equal(expectedAppliedScore, productRating.Score);
        }
        finally
        {
            await harness.Stop();
        }
    }
}