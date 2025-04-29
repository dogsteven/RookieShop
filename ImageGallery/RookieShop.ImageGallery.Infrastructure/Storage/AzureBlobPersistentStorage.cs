using Azure.Storage.Blobs;
using RookieShop.ImageGallery.Application.Abstractions;

namespace RookieShop.ImageGallery.Infrastructure.Storage;

public class AzureBlobPersistentStorage : IPersistentStorage
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureBlobPersistentStorage(BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient;
    }

    public async Task<Stream> ReadAsync(Guid id, CancellationToken cancellationToken)
    {
        var client = _blobContainerClient.GetBlobClient(id.ToString());

        return await client.OpenReadAsync(cancellationToken: cancellationToken);
    }

    public async Task SaveAsync(Guid id, Stream stream, CancellationToken cancellationToken)
    {
        await _blobContainerClient.UploadBlobAsync(id.ToString(), stream, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
        
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}