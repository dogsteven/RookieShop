using Microsoft.EntityFrameworkCore;
using Pgvector;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.ProductCatalog.Application.Queries;

public class ProductQueryService
{
    private readonly ProductCatalogDbContext _dbContext;
    private readonly ISemanticEncoder _semanticEncoder;

    public ProductQueryService(ProductCatalogDbContext dbContext, ISemanticEncoder semanticEncoder)
    {
        _dbContext = dbContext;
        _semanticEncoder = semanticEncoder;
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
            Rating = new ProductRatingDto
            {
                Score = product.Rating.Score,
                OneCount = product.Rating.OneCount,
                TwoCount = product.Rating.TwoCount,
                ThreeCount = product.Rating.ThreeCount,
                FourCount = product.Rating.FourCount,
                FiveCount = product.Rating.FiveCount
            },
            AvailableQuantity = product.StockLevel.AvailableQuantity
        };
    }
    
    public virtual async Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .Include(product => product.StockLevel)
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
            .Include(product => product.StockLevel)
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
            .Include(product => product.StockLevel)
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
            .Include(product => product.StockLevel)
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

    public virtual async Task<Pagination<ProductDto>> GetSemanticallyOrderedProductsAsync(string semantic,
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var semanticVector = new Vector(await _semanticEncoder.EncodeAsync(semantic, cancellationToken));

        var products = await _dbContext.GetSemanticallyOrderedProductsAsync(semanticVector, (pageNumber - 1) * pageSize, pageSize, cancellationToken);
        
        var count = await _dbContext.Products.LongCountAsync(cancellationToken);

        return new Pagination<ProductDto>
        {
            Count = count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = products.Select(Map)
        };
    }
}