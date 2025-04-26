namespace RookieShop.ImageGallery.Application.Abstractions;

public interface IPersistentStorage
{
    public Task<Stream> ReadAsync(Guid id, CancellationToken cancellationToken = default);
    public Task SaveAsync(Guid id, Stream stream, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}