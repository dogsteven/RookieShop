using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.Infrastructure.Persistence;

namespace RookieShop.ProductCatalog.Infrastructure.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductCatalog(this IServiceCollection services,
        Action<ProductCatalogConfigurator> configure)
    {
        var configurator = new ProductCatalogConfigurator();
        configure(configurator);
        
        return configurator.ConfigureServices(services);
    }
}

public class ProductCatalogConfigurator
{
    private Func<IServiceProvider, string>? _databaseConnectionString;
    private string? _migrationAssembly;

    internal ProductCatalogConfigurator()
    {
        _databaseConnectionString = null;
        _migrationAssembly = null;
    }

    public ProductCatalogConfigurator SetDatabaseConnectionString(Func<IServiceProvider, string>? databaseConnectionString)
    {
        _databaseConnectionString = databaseConnectionString; return this;
    }

    public ProductCatalogConfigurator SetMigrationAssembly(string? migrationAssembly)
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

        services.AddDbContext<ProductCatalogDbContextImpl>((provider, db) =>
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

        services.AddScoped<ProductCatalogDbContext, ProductCatalogDbContextImpl>();
        
        services.AddScoped<ProductQueryService>();
        services.AddScoped<CategoryQueryService>();
        
        return services;
    }
}