using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Application.Queries;
using RookieShop.ImageGallery.Infrastructure.Persistence;
using RookieShop.ImageGallery.Infrastructure.Storage;

namespace RookieShop.ImageGallery.Infrastructure.Configurations;

public static class ImageGalleryServiceCollectionExtensions
{
    public static IServiceCollection AddImageGallery(this IServiceCollection services,
        Action<ImageGalleryConfigurator> configure)
    {
        var configurator = new ImageGalleryConfigurator();
        configure(configurator);
        
        return configurator.ConfigureServices(services);
    }
}

public class ImageGalleryConfigurator
{
    private Func<IServiceProvider, string>? _databaseConnectionString;
    private string? _migrationAssembly;
    
    private Func<IServiceProvider, BlobContainerClient>? _blobContainerClientFactory; 

    internal ImageGalleryConfigurator()
    {
        _databaseConnectionString = null;
        _migrationAssembly = null;
        
        _blobContainerClientFactory = null;
    }

    public ImageGalleryConfigurator SetDatabaseConnectionString(Func<IServiceProvider, string>? databaseConnectionString)
    {
        _databaseConnectionString = databaseConnectionString;
        return this;
    }

    public ImageGalleryConfigurator SetMigrationAssembly(string? migrationAssembly)
    {
        _migrationAssembly = migrationAssembly;
        return this;
    }

    public ImageGalleryConfigurator SetBlobContainerClient(Func<IServiceProvider, BlobContainerClient>? blobContainerClientFactory)
    {
        _blobContainerClientFactory = blobContainerClientFactory;
        return this;
    }

    internal IServiceCollection ConfigureServices(IServiceCollection services)
    {
        if (_databaseConnectionString == null || _blobContainerClientFactory == null)
        {
            return services;
        }

        services.AddDbContext<ImageGalleryDbContextImpl>((provider, db) =>
        {
            var connectionString = _databaseConnectionString(provider);

            db.UseNpgsql(connectionString, npgsql =>
            {
                if (_migrationAssembly != null)
                {
                    npgsql.MigrationsAssembly(_migrationAssembly);
                }

                npgsql.UseVector();
            });
        });

        services.AddScoped<ImageGalleryDbContext>(provider => provider.GetRequiredService<ImageGalleryDbContextImpl>());
        
        services.AddSingleton<ITemporaryStorage, OperatingSystemTemporaryStorage>();
        services.AddSingleton<IPersistentStorage>(provider =>
        {
            var blobContainerClient = _blobContainerClientFactory(provider);
            
            return new AzureBlobPersistentStorage(blobContainerClient);
        });

        services.AddScoped<ImageQueryService>();
        
        return services;
    }
}