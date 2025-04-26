namespace RookieShop.ImageGallery.Abstractions;

public interface IPersistentStorage<in TKey>
{
    public Task<Stream> GetImageByIdAsync(TKey key, CancellationToken cancellationToken = default);
    public Task SaveImageAsync(TKey key, Stream stream, CancellationToken cancellationToken = default);
    public Task DeleteImageAsync(TKey key, CancellationToken cancellationToken = default);
}