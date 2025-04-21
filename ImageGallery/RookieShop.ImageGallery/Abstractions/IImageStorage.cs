namespace RookieShop.ImageGallery.Abstractions;

public interface IImageStorage
{
    public Task<Stream> GetImageByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task SaveImageAsync(Guid id, Stream stream, CancellationToken cancellationToken);
    public Task DeleteImageAsync(Guid id, CancellationToken cancellationToken);
}