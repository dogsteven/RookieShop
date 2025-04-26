using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Application.Exceptions;
using RookieShop.ImageGallery.Application.Models;
using RookieShop.Shared.Models;

namespace RookieShop.ImageGallery.Application.Queries;

public class ImageQueryService
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly ITemporaryStorage _temporaryStorage;
    private readonly IPersistentStorage _persistentStorage;

    public ImageQueryService(ImageGalleryDbContext dbContext, ITemporaryStorage temporaryStorage, IPersistentStorage persistentStorage)
    {
        _dbContext = dbContext;
        _temporaryStorage = temporaryStorage;
        _persistentStorage = persistentStorage;
    }

    public async Task<Pagination<ImageDto>> GetImagesAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Images
            .OrderByDescending(image => image.CreatedDate)
            .Select(image => new ImageDto(image))
            .AsNoTracking();
        
        var images = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var count = await query.LongCountAsync(cancellationToken);

        return new Pagination<ImageDto>
        {
            Count = count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = images
        };
    }

    public async Task<(Stream, string)> OpenStreamAsync(Guid id, CancellationToken cancellationToken)
    {
        var image = await _dbContext.Images.AsNoTracking()
            .Where(image => image.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (image == null)
        {
            throw new ImageNotFoundException(id);
        }

        Stream stream;

        if (!image.IsSynced)
        {
            stream = await _temporaryStorage.ReadAsync(image.TemporaryEntryId, cancellationToken);
        }
        else
        {
            stream = await _persistentStorage.ReadAsync(image.Id, cancellationToken);
        }

        return (stream, image.ContentType);
    }
}