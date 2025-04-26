namespace RookieShop.ImageGallery.Abstractions;

public interface ITemporaryStorage
{
    public Task<Stream> OpenAsStreamAsync(string path, CancellationToken cancellationToken = default);
    public Task<string> SaveStreamAsync(Stream stream, CancellationToken cancellationToken = default);
    public Task DeleteAsync(string path, CancellationToken cancellationToken = default);
}