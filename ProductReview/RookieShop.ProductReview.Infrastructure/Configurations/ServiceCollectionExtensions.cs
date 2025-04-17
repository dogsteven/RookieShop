using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.ProductReview.Application.Abstractions;
using RookieShop.ProductReview.Application.Queries;
using RookieShop.ProductReview.Infrastructure.Persistence;

namespace RookieShop.ProductReview.Infrastructure.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductReview(this IServiceCollection services,
        Action<ProductReviewConfigurator> configure)
    {
        var configurator = new ProductReviewConfigurator();
        configure(configurator);
        
        return configurator.ConfigureServices(services);
    }
}

public class ProductReviewConfigurator
{
    private Func<IServiceProvider, string>? _databaseConnectionString;
    private string? _migrationAssembly;

    internal ProductReviewConfigurator()
    {
        _databaseConnectionString = null;
        _migrationAssembly = null;
    }

    public ProductReviewConfigurator SetDatabaseConnectionString(Func<IServiceProvider, string>? databaseConnectionString)
    {
        _databaseConnectionString = databaseConnectionString; return this;
    }

    public ProductReviewConfigurator SetMigrationAssembly(string? migrationAssembly)
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

        services.AddDbContext<ProductReviewDbContextImpl>((provider, db) =>
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

        services.AddScoped<ProductReviewDbContext, ProductReviewDbContextImpl>();
        
        services.AddScoped<ReviewQueryService>();
        
        return services;
    }
}