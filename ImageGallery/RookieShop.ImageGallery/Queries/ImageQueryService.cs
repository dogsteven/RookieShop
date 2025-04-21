using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Abstractions;
using RookieShop.ImageGallery.Entities;
using RookieShop.ImageGallery.Exceptions;
using RookieShop.ImageGallery.Models;
using RookieShop.Shared.Models;

namespace RookieShop.ImageGallery.Queries;

public class ImageQueryService
{
    private readonly ImageGalleryDbContext _dbContext;
    private readonly IImageStorage _imageStorage;

    public ImageQueryService(ImageGalleryDbContext dbContext, IImageStorage imageStorage)
    {
        _dbContext = dbContext;
        _imageStorage = imageStorage;
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

    public async Task<(Stream, string)> GetImageByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var image = await _dbContext.Images.AsNoTracking()
            .Where(image => image.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (image == null)
        {
            throw new ImageNotFoundException(id);
        }

        Stream stream;

        if (!image.IsUploaded)
        {
            stream = new FileStream(image.TempFileName, FileMode.Open);
        }
        else
        {
            stream = await _imageStorage.GetImageByIdAsync(image.Id, cancellationToken);
        }

        return (stream, image.ContentType);
    }
}