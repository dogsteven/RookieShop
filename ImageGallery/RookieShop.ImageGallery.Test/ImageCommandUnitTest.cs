using MassTransit.Mediator;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Application.Commands;
using RookieShop.ImageGallery.Application.Entities;
using RookieShop.ImageGallery.Application.Events;
using RookieShop.ImageGallery.Application.Exceptions;
using RookieShop.ImageGallery.Test.Utilities;

namespace RookieShop.ImageGallery.Test;

public class ImageCommandUnitTest
{
    [Fact]
    public async Task Test_UploadImage()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        var mockTemporaryStorage = provider.GetRequiredService<Mock<ITemporaryStorage>>();
        
        var temporaryEntryId = Guid.NewGuid().ToString();
        
        mockTemporaryStorage.Setup(temporaryStorage => temporaryStorage.SaveAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(temporaryEntryId);
        
        using var scope = provider.CreateScope();

        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        
        var id = Guid.NewGuid();
        var contentType = "image/png";
        await using var stream = new MemoryStream([6]);

        var uploadImage = new UploadImage
        {
            Id = id,
            ContentType = contentType,
            Stream = stream,
        };

        try
        {
            await harness.Start();
            
            // Act
            await scopedMediator.Send(uploadImage);
            
            // Assert
            
            // Assert temporary storage has called Save once
            mockTemporaryStorage.Verify(temporaryStorage => temporaryStorage.SaveAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
            
            // Assert image has been persisted
            var context = scope.ServiceProvider.GetRequiredService<ImageGalleryDbContext>();
            
            var image = await context.Images.FirstOrDefaultAsync(image => image.Id == id);
            
            Assert.NotNull(image);
            Assert.Equal(temporaryEntryId, image.TemporaryEntryId);
            Assert.False(image.IsSynced);
            
            // Assert ImageUploaded has been published
            Assert.True(await harness.Published.Any<ImageUploaded>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Test_SyncTemporaryEntryToPersistentStorage()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        
        var id = Guid.NewGuid();
        var temporaryEntryId = Guid.NewGuid().ToString();
        
        using (var prepareScope = provider.CreateScope())
        {
            var uploadedImage = new Image(id, "image/png", temporaryEntryId);
            
            var context = prepareScope.ServiceProvider.GetRequiredService<ImageGalleryDbContext>();
            
            context.Images.Add(uploadedImage);
            
            await context.SaveChangesAsync();
        }
        
        using var scope = provider.CreateScope();

        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        try
        {
            await harness.Start();
            
            // Act
            await harness.Bus.Publish(new ImageUploaded
            {
                Id = id
            });
            
            // Assert
            
            // Assert ImageUploaded has been consumed by the right consumer
            var syncTemporaryEntryToPersistentStorageConsumer = harness.GetConsumerHarness<SyncTemporaryEntryToPersistentStorageConsumer>();
            
            Assert.True(await syncTemporaryEntryToPersistentStorageConsumer.Consumed.Any<ImageUploaded>());
            
            // Assert ImageSynced has been published
            Assert.True(await harness.Published.Any<ImageSynced>());
            
            // Assert image has been marked as synced
            var context = scope.ServiceProvider.GetRequiredService<ImageGalleryDbContext>();
            
            var image = await context.Images.FirstOrDefaultAsync(image => image.Id == id);
            
            Assert.NotNull(image);
            Assert.True(image.IsSynced);
            
            // Assert temporary storage has called Read(temporaryEntryId) once 
            var mockTemporaryStorage = scope.ServiceProvider.GetRequiredService<Mock<ITemporaryStorage>>();
            
            mockTemporaryStorage.Verify(temporaryStorage => temporaryStorage.ReadAsync(temporaryEntryId, It.IsAny<CancellationToken>()), Times.Once);
            
            // Assert persistent storage has called Save(id) once
            var mockPersistentStorage = scope.ServiceProvider.GetRequiredService<Mock<IPersistentStorage>>();
            
            mockPersistentStorage.Verify(persistentStorage => persistentStorage.SaveAsync(id, It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await harness.Stop();
        }
    }
    
    [Fact]
    public async Task Test_CleanUpTemporaryStorageOnSyncedConsumer()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        var id = Guid.NewGuid();
        var temporaryEntryId = Guid.NewGuid().ToString();
        
        using (var prepareScope = provider.CreateScope())
        {
            var syncedImage = new Image(id, "image/png", temporaryEntryId);
            syncedImage.MarkAsSynced();
            
            var context = prepareScope.ServiceProvider.GetRequiredService<ImageGalleryDbContext>();
            
            context.Images.Add(syncedImage);
            
            await context.SaveChangesAsync();
        }
        
        using var scope = provider.CreateScope();

        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        try
        {
            await harness.Start();
            
            // Act
            await harness.Bus.Publish(new ImageSynced
            {
                Id = id
            });
            
            // Assert
            
            // Assert ImageSynced has been consumed by the right consumer
            var cleanUpTemporaryStorageOnSyncedConsumer = harness.GetConsumerHarness<CleanUpTemporaryStorageOnSyncedConsumer>();
            
            Assert.True(await cleanUpTemporaryStorageOnSyncedConsumer.Consumed.Any<ImageSynced>());
            
            // Assert temporary storage has called Delete(temporaryEntryId) once 
            var mockTemporaryStorage = scope.ServiceProvider.GetRequiredService<Mock<ITemporaryStorage>>();
            
            mockTemporaryStorage.Verify(temporaryStorage => temporaryStorage.DeleteAsync(temporaryEntryId, It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Should_DeleteImage_FailedWithNotFoundImage()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        
        var deleteImage = new DeleteImage
        {
            Id = Guid.NewGuid()
        };
        
        // Act
        var deleteImageAction = async () => await scopedMediator.Send(deleteImage);
        
        // Assert
        await Assert.ThrowsAsync<ImageNotFoundException>(deleteImageAction);
    }
    
    [Fact]
    public async Task Should_DeleteImage_Success()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        var id = Guid.NewGuid();
        
        using (var prepareScope = provider.CreateScope())
        {
            var syncedImage = new Image(id, "image/png", Guid.NewGuid().ToString());
            syncedImage.MarkAsSynced();
            
            var context = prepareScope.ServiceProvider.GetRequiredService<ImageGalleryDbContext>();
            
            context.Images.Add(syncedImage);
            
            await context.SaveChangesAsync();
        }
        
        using var scope = provider.CreateScope();
        
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        
        var deleteImage = new DeleteImage
        {
            Id = id
        };

        try
        {
            await harness.Start();
            
            // Act
            await scopedMediator.Send(deleteImage);

            // Assert
            
            // Assert image has been deleted
            var context = scope.ServiceProvider.GetRequiredService<ImageGalleryDbContext>();
            
            var image = await context.Images.FirstOrDefaultAsync(image => image.Id == id);
            
            Assert.Null(image);
            
            // Assert ImageDeleted has been published
            Assert.True(await harness.Published.Any<ImageDeleted>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Test_CleanUpPersistentStorageOnDeletedConsumer()
    {
        // Arrange
        var services = new ImageGalleryServiceCollection();

        var provider = services.BuildServiceProvider();
        
        using var scope = provider.CreateScope();
        
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        var id = Guid.NewGuid();
        
        try
        {
            await harness.Start();
            
            // Act
            await harness.Bus.Publish(new ImageDeleted
            {
                Id = id
            });

            // Assert
            
            // Assert ImageDeleted has been consumed by the right consumer
            var cleanUpPersistentStorageOnDeletedConsumer = harness.GetConsumerHarness<CleanUpPersistentStorageOnDeletedConsumer>();
            
            Assert.True(await cleanUpPersistentStorageOnDeletedConsumer.Consumed.Any<ImageDeleted>());
            
            // Assert persistent storage has called Delete(id) once
            var mockPersistentStorage = scope.ServiceProvider.GetRequiredService<Mock<IPersistentStorage>>();
            
            mockPersistentStorage.Verify(storage => storage.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await harness.Stop();
        }
    }
}