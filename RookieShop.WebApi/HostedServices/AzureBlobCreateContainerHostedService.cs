using Azure.Storage.Blobs;

namespace RookieShop.WebApi.HostedServices;

public class AzureBlobCreateContainerHostedService : BackgroundService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobCreateContainerHostedService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("rookie-shop-images");

        await containerClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
    }
}