using RookieShop.ImageGallery.Application.Abstractions;

namespace RookieShop.ImageGallery.Infrastructure.Storage;

public class OperatingSystemTemporaryStorage : ITemporaryStorage
{
    public Task<Stream> ReadAsync(string path, CancellationToken cancellationToken)
    {
        return Task.FromResult<Stream>(new FileStream(path, FileMode.Open));
    }

    public async Task<string> SaveAsync(Stream stream, CancellationToken cancellationToken)
    {
        var tempFileName = Path.GetTempFileName();

        await using var fileStream = new FileStream(tempFileName, FileMode.Create);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return tempFileName;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        
        return Task.CompletedTask;
    }
}