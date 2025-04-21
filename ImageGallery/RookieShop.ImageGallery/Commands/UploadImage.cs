using MassTransit;
using RookieShop.ImageGallery.Abstractions;
using RookieShop.ImageGallery.Entities;
using RookieShop.ImageGallery.Events;

namespace RookieShop.ImageGallery.Commands;

public class UploadImage
{
    public string ContentType { get; set; } = null!;
    
    public Stream Stream { get; set; } = null!;
}

public class UploadImageConsumer : IConsumer<UploadImage>
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public UploadImageConsumer(ImageGalleryDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<UploadImage> context)
    {
        var message = context.Message;
        var contentType = message.ContentType;
        var stream = message.Stream;
        
        var cancellationToken = context.CancellationToken;

        var tempFileName = Path.GetTempFileName();

        await using (var fileStream = new FileStream(tempFileName, FileMode.Create))
        {
            await stream.CopyToAsync(fileStream, cancellationToken);
        }
        
        var id = Guid.NewGuid();
        var image = new Image(id, contentType, tempFileName);
        
        _dbContext.Images.Add(image);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new ImageUploaded
        {
            Id = id,
        }, cancellationToken);
    }
}