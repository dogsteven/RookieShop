namespace RookieShop.ImageGallery.Application.Abstractions;

public interface ITemporaryStorage
{
    public Task<Stream> ReadAsync(string id, CancellationToken cancellationToken = default);
    
    public Task<string> SaveAsync(Stream stream, CancellationToken cancellationToken = default);
    
    public Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}