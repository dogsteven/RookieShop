namespace RookieShop.ImageGallery.Test.Utilities;

public static class StreamExtensions
{
    public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        
        return memoryStream.ToArray();
    }
}