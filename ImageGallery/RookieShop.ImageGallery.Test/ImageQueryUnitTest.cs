using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Application.Entities;
using RookieShop.ImageGallery.Application.Exceptions;
using RookieShop.ImageGallery.Application.Queries;
using RookieShop.ImageGallery.Test.Utilities;

namespace RookieShop.ImageGallery.Test;

public class ImageQueryUnitTest
{
    [Fact]
    public async Task Should_OpenStreamAsync_FailedWithNotFoundImage()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();

        var imageQueryService = scope.ServiceProvider.GetRequiredService<ImageQueryService>();
        
        // Act
        var openStreamAsyncAction = async () => await imageQueryService.OpenStreamAsync(Guid.NewGuid(), default);
        
        // Assert
        await Assert.ThrowsAsync<ImageNotFoundException>(openStreamAsyncAction);
    }
    
    [Fact]
    public async Task Should_OpenStreamAsync_SuccessWithTemporaryStorageRead()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        var id = Guid.NewGuid();
        var temporaryEntryId = Guid.NewGuid().ToString();

        using (var prepareScope = provider.CreateScope())
        {
            var context = prepareScope.ServiceProvider.GetRequiredService<ImageGalleryDbContext>();
            
            var hasNotBeenSyncedImage = new Image(id, "image/png", temporaryEntryId);
            
            context.Images.Add(hasNotBeenSyncedImage);
            
            await context.SaveChangesAsync();
        }
        
        using var scope = provider.CreateScope();

        var imageQueryService = scope.ServiceProvider.GetRequiredService<ImageQueryService>();
        
        // Act
        await using var _ = (await imageQueryService.OpenStreamAsync(id, default)).Item1;

        // Assert
        var mockTemporaryStorage = scope.ServiceProvider.GetRequiredService<Mock<ITemporaryStorage>>();
        
        mockTemporaryStorage.Verify(temporaryStorage => temporaryStorage.ReadAsync(temporaryEntryId, It.IsAny<CancellationToken>()), Times.Once);
        
        var mockPersistentStorage = scope.ServiceProvider.GetRequiredService<Mock<IPersistentStorage>>();
        
        mockPersistentStorage.Verify(persistentStorage => persistentStorage.ReadAsync(id, It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Should_OpenStreamAsync_SuccessWithPersistentStorageRead()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        var id = Guid.NewGuid();
        var temporaryEntryId = Guid.NewGuid().ToString();

        using (var prepareScope = provider.CreateScope())
        {
            var context = prepareScope.ServiceProvider.GetRequiredService<ImageGalleryDbContext>();
            
            var syncedImage = new Image(id, "image/png", temporaryEntryId);
            syncedImage.MarkAsSynced();
            
            context.Images.Add(syncedImage);
            
            await context.SaveChangesAsync();
        }
        
        using var scope = provider.CreateScope();

        var imageQueryService = scope.ServiceProvider.GetRequiredService<ImageQueryService>();
        
        // Act
        await using var _ = (await imageQueryService.OpenStreamAsync(id, default)).Item1;

        // Assert
        var mockTemporaryStorage = scope.ServiceProvider.GetRequiredService<Mock<ITemporaryStorage>>();
        
        mockTemporaryStorage.Verify(temporaryStorage => temporaryStorage.ReadAsync(temporaryEntryId, It.IsAny<CancellationToken>()), Times.Never);
        
        var mockPersistentStorage = scope.ServiceProvider.GetRequiredService<Mock<IPersistentStorage>>();
        
        mockPersistentStorage.Verify(persistentStorage => persistentStorage.ReadAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}