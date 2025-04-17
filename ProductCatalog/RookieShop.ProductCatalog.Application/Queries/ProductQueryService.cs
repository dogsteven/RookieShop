using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Application.Models;

namespace RookieShop.ProductCatalog.Application.Queries;

public class ProductQueryService
{
    private readonly ProductCatalogDbContext _dbContext;

    public ProductQueryService(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .Where(p => p.Sku == sku)
            .Select(product => new ProductDto(product))
            .AsNoTracking();

        var productDto = await query.FirstOrDefaultAsync(cancellationToken);

        if (productDto == null)
        {
            throw new ProductNotFoundException(sku);
        }
        
        return productDto;
    }

    public async Task<Pagination<ProductDto>> GetProductsAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .OrderByDescending(product => product.UpdatedDate)
            .Select(product => new ProductDto(product))
            .AsNoTracking();
        
        var productDtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var count = await query.LongCountAsync(cancellationToken);

        return new Pagination<ProductDto>
        {
            Count = count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = productDtos
        };
    }

    public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int maxCount,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .Where(product => product.IsFeatured)
            .OrderByDescending(product => product.UpdatedDate)
            .Select(product => new ProductDto(product))
            .AsNoTracking();
        
        var productDtos = await query
            .Take(maxCount)
            .ToListAsync(cancellationToken);

        return productDtos;
    }

    public async Task<Pagination<ProductDto>> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .Where(product => product.Category.Id == categoryId)
            .OrderByDescending(product => product.UpdatedDate)
            .Select(product => new ProductDto(product))
            .AsNoTracking();
        
        var productDtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var count = await query.LongCountAsync(cancellationToken);

        return new Pagination<ProductDto>
        {
            Count = count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = productDtos
        };
    }
}