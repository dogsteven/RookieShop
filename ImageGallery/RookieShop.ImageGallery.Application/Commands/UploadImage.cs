using MassTransit;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Application.Entities;
using RookieShop.ImageGallery.Application.Events;

namespace RookieShop.ImageGallery.Application.Commands;

public class UploadImage
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = null!;
    
    public Stream Stream { get; set; } = null!;
}

public class UploadImageConsumer : IConsumer<UploadImage>
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly ITemporaryStorage _temporaryStorage;
    private readonly IPublishEndpoint _publishEndpoint;

    public UploadImageConsumer(ImageGalleryDbContext dbContext, ITemporaryStorage temporaryStorage, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _temporaryStorage = temporaryStorage;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<UploadImage> context)
    {
        var message = context.Message;
        var id = message.Id;
        var contentType = message.ContentType;
        var stream = message.Stream;
        
        var cancellationToken = context.CancellationToken;

        var temporaryEntryId = await _temporaryStorage.SaveAsync(stream, cancellationToken);
        
        var image = new Image(id, contentType, temporaryEntryId);
        
        _dbContext.Images.Add(image);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new ImageUploaded
        {
            Id = id,
        }, cancellationToken);
    }
}