using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Application.Abstractions;

namespace RookieShop.ImageGallery.Application.Events;

public class ImageUploaded
{
    public Guid Id { get; set; }
}

public class SyncTemporaryEntryToPersistentStorageConsumer : IConsumer<ImageUploaded>
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly ITemporaryStorage _temporaryStorage;
    private readonly IPersistentStorage _persistentStorage;

    public SyncTemporaryEntryToPersistentStorageConsumer(ImageGalleryDbContext dbContext, ITemporaryStorage temporaryStorage, IPersistentStorage persistentStorage)
    {
        _dbContext = dbContext;
        _temporaryStorage = temporaryStorage;
        _persistentStorage = persistentStorage;
    }
    
    public async Task Consume(ConsumeContext<ImageUploaded> context)
    {
        var id = context.Message.Id;

        var cancellationToken = context.CancellationToken;
        
        var image = await _dbContext.Images
            .FirstOrDefaultAsync(image => image.Id == id, cancellationToken);

        if (image == null)
        {
            return;
        }
        
        await using var stream = await _temporaryStorage.ReadAsync(image.TemporaryEntryId, cancellationToken);
        
        await _persistentStorage.SaveAsync(id, stream, cancellationToken);
        
        image.MarkAsSynced();
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await context.Publish(new ImageSynced
        {
            Id = image.Id
        }, cancellationToken);
    }
}