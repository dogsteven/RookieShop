using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Application.Models;
using RookieShop.Shared.Models;

namespace RookieShop.ProductCatalog.Application.Queries;

public class ProductQueryService
{
    private readonly ProductCatalogDbContext _dbContext;

    public ProductQueryService(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static ProductDto Map(Product product)
    {
        return new ProductDto
        {
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.Category.Id,
            CategoryName = product.Category.Name,
            PrimaryImageId = product.PrimaryImageId,
            SupportingImageIds = product.SupportingImageIds,
            IsFeatured = product.IsFeatured,
            CreatedDate = product.CreatedDate,
            UpdatedDate = product.UpdatedDate,
            Rating = new RatingDto
            {
                Score = product.Rating.Score,
                OneCount = product.Rating.OneCount,
                TwoCount = product.Rating.TwoCount,
                ThreeCount = product.Rating.ThreeCount,
                FourCount = product.Rating.FourCount,
                FiveCount = product.Rating.FiveCount
            }
        };
    }
    
    public virtual async Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .Where(p => p.Sku == sku)
            .Select(product => Map(product))
            .AsNoTracking();

        var productDto = await query.FirstOrDefaultAsync(cancellationToken);

        if (productDto == null)
        {
            throw new ProductNotFoundException(sku);
        }
        
        return productDto;
    }

    public virtual async Task<Pagination<ProductDto>> GetProductsAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .OrderByDescending(product => product.UpdatedDate)
            .Select(product => Map(product))
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

    public virtual async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int maxCount,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .Where(product => product.IsFeatured)
            .OrderByDescending(product => product.UpdatedDate)
            .Select(product => Map(product))
            .AsNoTracking();
        
        var productDtos = await query
            .Take(maxCount)
            .ToListAsync(cancellationToken);

        return productDtos;
    }

    public virtual async Task<Pagination<ProductDto>> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .Where(product => product.Category.Id == categoryId)
            .OrderByDescending(product => product.UpdatedDate)
            .Select(product => Map(product))
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