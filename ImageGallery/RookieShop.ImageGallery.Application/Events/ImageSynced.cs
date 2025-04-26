using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Application.Abstractions;

namespace RookieShop.ImageGallery.Application.Events;

public class ImageSynced
{
    public Guid Id { get; set; }
}

public class CleanUpTemporaryStorageOnSyncedConsumer : IConsumer<ImageSynced>
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly ITemporaryStorage _temporaryStorage;

    public CleanUpTemporaryStorageOnSyncedConsumer(ImageGalleryDbContext dbContext, ITemporaryStorage temporaryStorage)
    {
        _dbContext = dbContext;
        _temporaryStorage = temporaryStorage;
    }
    
    public async Task Consume(ConsumeContext<ImageSynced> context)
    {
        var id = context.Message.Id;
        
        var cancellationToken = context.CancellationToken;
        
        var image = await _dbContext.Images.AsNoTracking()
            .FirstOrDefaultAsync(image => image.Id == id, cancellationToken);

        if (image == null)
        {
            return;
        }
        
        await _temporaryStorage.DeleteAsync(image.TemporaryEntryId, cancellationToken);
    }
}