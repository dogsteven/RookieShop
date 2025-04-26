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

    internal ImageGalleryConfigurator()
    {
        _databaseConnectionString = null;
        _migrationAssembly = null;
    }

    public ImageGalleryConfigurator SetDatabaseConnectionString(Func<IServiceProvider, string>? databaseConnectionString)
    {
        _databaseConnectionString = databaseConnectionString; return this;
    }

    public ImageGalleryConfigurator SetMigrationAssembly(string? migrationAssembly)
    {
        _migrationAssembly = migrationAssembly;
        return this;
    }

    internal IServiceCollection ConfigureServices(IServiceCollection services)
    {
        if (_databaseConnectionString == null)
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
            });
        });

        services.AddScoped<ImageGalleryDbContext, ImageGalleryDbContextImpl>();
        
        services.AddSingleton<ITemporaryStorage, OperatingSystemTemporaryStorage>();
        services.AddSingleton<IPersistentStorage, AzureBlobPersistentStorage>();

        services.AddScoped<ImageQueryService>();
        
        return services;
    }
}