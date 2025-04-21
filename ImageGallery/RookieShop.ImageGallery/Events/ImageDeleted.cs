using MassTransit;
using RookieShop.ImageGallery.Abstractions;

namespace RookieShop.ImageGallery.Events;

public class ImageDeleted
{
    public Guid Id { get; set; }
}

public class DeleteImageFromStorageConsumer : IConsumer<ImageDeleted>
{
    private readonly IImageStorage _imageStorage;

    public DeleteImageFromStorageConsumer(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }
    
    public async Task Consume(ConsumeContext<ImageDeleted> context)
    {
        var id = context.Message.Id;
        
        var cancellationToken = context.CancellationToken;

        await _imageStorage.DeleteImageAsync(id, cancellationToken);
    }
}