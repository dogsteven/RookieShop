using MassTransit;
using RookieShop.ImageGallery.Application.Abstractions;

namespace RookieShop.ImageGallery.Application.Events;

public class ImageDeleted
{
    public Guid Id { get; set; }
}

public class CleanUpPersistentStorageOnDeletedConsumer : IConsumer<ImageDeleted>
{
    private readonly IPersistentStorage _persistentStorage;

    public CleanUpPersistentStorageOnDeletedConsumer(IPersistentStorage persistentStorage)
    {
        _persistentStorage = persistentStorage;
    }
    
    public async Task Consume(ConsumeContext<ImageDeleted> context)
    {
        var id = context.Message.Id;
        
        var cancellationToken = context.CancellationToken;

        await _persistentStorage.DeleteAsync(id, cancellationToken);
    }
}