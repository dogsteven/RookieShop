using Azure.Storage.Blobs;
using RookieShop.ImageGallery.Abstractions;

namespace RookieShop.ImageGallery.Infrastructure.ImageStorage;

public class AzureBlobImageStorage : IImageStorage
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureBlobImageStorage(BlobServiceClient blobServiceClient)
    {
        _blobContainerClient = blobServiceClient.GetBlobContainerClient("rookie-shop-images");
    }

    public async Task<Stream> GetImageByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var client = _blobContainerClient.GetBlobClient(id.ToString());

        return await client.OpenReadAsync(cancellationToken: cancellationToken);
    }

    public async Task SaveImageAsync(Guid id, Stream stream, CancellationToken cancellationToken)
    {
        await _blobContainerClient.UploadBlobAsync(id.ToString(), stream, cancellationToken: cancellationToken);
    }

    public async Task DeleteImageAsync(Guid id, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
        
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}