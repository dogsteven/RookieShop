using Microsoft.EntityFrameworkCore;
using RookieShop.Application.Abstractions;
using RookieShop.Application.Entities;
using RookieShop.Application.Exceptions;
using RookieShop.Application.Models;

namespace RookieShop.Application.Services;

public class ProductService
{
    private readonly RookieShopDbContext _dbContext;

    public ProductService(RookieShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static ProductDto Map(Product product, float ratingScore, int ratingCount)
    {
        return new ProductDto
        {
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.Category.Id,
            CategoryName = product.Category.Name,
            ImageUrl = product.ImageUrl,
            IsFeatured = product.IsFeatured,
            CreatedDate = product.CreatedDate,
            UpdatedDate = product.UpdatedDate,
            RatingScore = ratingScore,
            RatingCount = ratingCount
        };
    }

    public async Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var query = (from product in _dbContext.Products.Include(p => p.Category)
            join rating in _dbContext.Ratings on product.Sku equals rating.Sku into ratings
            where product.Sku == sku
            select Map(product, ratings.Any() ? ratings.Average(r => r.Score) : 0f, ratings.Count())).AsNoTracking();

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
        var query = (from product in _dbContext.Products.Include(p => p.Category)
            join rating in _dbContext.Ratings on product.Sku equals rating.Sku into ratings
            orderby product.UpdatedDate descending
            select Map(product, ratings.Any() ? ratings.Average(r => r.Score) : 0f, ratings.Count())).AsNoTracking();
        
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
        var query = (from product in _dbContext.Products.Include(p => p.Category)
            join rating in _dbContext.Ratings on product.Sku equals rating.Sku into ratings
            where product.IsFeatured
            orderby product.UpdatedDate descending
            select Map(product, ratings.Any() ? ratings.Average(r => r.Score) : 0f, ratings.Count())).AsNoTracking();
        
        var productDtos = await query
            .Take(maxCount)
            .ToListAsync(cancellationToken);

        return productDtos;
    }

    public async Task<Pagination<ProductDto>> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = (from product in _dbContext.Products.Include(p => p.Category)
            join rating in _dbContext.Ratings on product.Sku equals rating.Sku into ratings
            where product.Category.Id == categoryId
            orderby product.UpdatedDate descending
            select Map(product, ratings.Any() ? ratings.Average(r => r.Score) : 0f, ratings.Count())).AsNoTracking();
        
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

    public async Task CreateProductAsync(string sku, string name, string description, decimal price, int categoryId,
        string imageUrl, bool isFeatured, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException(categoryId);
        }

        var now = DateTime.UtcNow;

        var product = new Product
        {
            Sku = sku,
            Name = name,
            Description = description,
            Price = price,
            Category = category,
            ImageUrl = imageUrl,
            IsFeatured = isFeatured,
            CreatedDate = now,
            UpdatedDate = now
        };
        
        _dbContext.Products.Add(product);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProductAsync(string sku, string name, string description, decimal price, int categoryId,
        string imageUrl, bool isFeatured, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.Include(product => product.Category)
            .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(sku);
        }

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.ImageUrl = imageUrl;
        product.IsFeatured = isFeatured;
        product.UpdatedDate = DateTime.UtcNow;

        if (product.Category.Id != categoryId)
        {
            var newCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);

            if (newCategory == null)
            {
                throw new CategoryNotFoundException(categoryId);
            }
            
            product.Category = newCategory;
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProductAsync(string sku, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(sku);
        }
        
        _dbContext.Products.Remove(product);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}