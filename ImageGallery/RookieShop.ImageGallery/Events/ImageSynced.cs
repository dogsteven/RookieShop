using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Abstractions;

namespace RookieShop.ImageGallery.Events;

public class ImageSynced
{
    public Guid Id { get; set; }
}

public class DeleteTemporaryImageOnSyncConsumer : IConsumer<ImageSynced>
{
    private readonly ImageGalleryDbContext _dbContext;

    public DeleteTemporaryImageOnSyncConsumer(ImageGalleryDbContext dbContext)
    {
        _dbContext = dbContext;
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
        
        if (File.Exists(image.TempFileName)) {
            File.Delete(image.TempFileName);
        }
    }
}