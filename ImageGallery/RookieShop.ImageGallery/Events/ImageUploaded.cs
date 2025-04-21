using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Abstractions;

namespace RookieShop.ImageGallery.Events;

public class ImageUploaded
{
    public Guid Id { get; set; }
}

public class UploadImageToStorageConsumer : IConsumer<ImageUploaded>
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly IImageStorage _imageStorage;

    public UploadImageToStorageConsumer(ImageGalleryDbContext dbContext, IImageStorage imageStorage)
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
        
        image.MarkAsUploaded();
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}