using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Application.Events;
using RookieShop.ImageGallery.Application.Exceptions;

namespace RookieShop.ImageGallery.Application.Commands;

public class DeleteImage
{
    public Guid Id { get; set; }
}

public class DeleteImageConsumer : IConsumer<DeleteImage>
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteImageConsumer(ImageGalleryDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<DeleteImage> context)
    {
        var id = context.Message.Id;
        
        var cancellationToken = context.CancellationToken;
        
        var image = await _dbContext.Images
            .FirstOrDefaultAsync(image => image.Id == id, cancellationToken);

        if (image == null)
        {
            throw new ImageNotFoundException(id);
        }
        
        _dbContext.Images.Remove(image);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new ImageDeleted
        {
            Id = id
        }, cancellationToken);
    }
}