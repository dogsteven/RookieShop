using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Abstractions;

namespace RookieShop.ImageGallery.Events;

public class ImageUploaded
{
    public Guid Id { get; set; }
}

public class SyncImageToStorageConsumer : IConsumer<ImageUploaded>
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly IImageStorage _imageStorage;

    public SyncImageToStorageConsumer(ImageGalleryDbContext dbContext, IImageStorage imageStorage)
    {
        _dbContext = dbContext;
        _imageStorage = imageStorage;
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
        
        await using var fileStream = new FileStream(image.TempFileName, FileMode.Open);
        
        await _imageStorage.SaveImageAsync(id, fileStream, cancellationToken);
        
        image.MarkAsSynced();
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await context.Publish(new ImageSynced
        {
            Id = image.Id
        }, cancellationToken);
    }
}