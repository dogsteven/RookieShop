using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Exceptions;
using RookieShop.ProductCatalog.Application.Models;

namespace RookieShop.ProductCatalog.Application.Queries;

public class CategoryQueryService
{
    private readonly ProductCatalogDbContext _dbContext;

    public CategoryQueryService(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        var query = (from category in _dbContext.Categories
            join product in _dbContext.Products on category.Id equals product.Category.Id into products
            select new CategoryDto
            {
                Id = category.Id, Name = category.Name, Description = category.Description,
                ProductCount = products.Count()
            }).AsNoTracking();
        
        var categoryDto = await query.FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (categoryDto == null)
        {
            throw new CategoryNotFoundException(id);
        }

        return categoryDto;
    }

    public virtual async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var query = (from category in _dbContext.Categories
            join product in _dbContext.Products on category.Id equals product.Category.Id into products
            select new CategoryDto
            {
                Id = category.Id, Name = category.Name, Description = category.Description,
                ProductCount = products.Count()
            }).AsNoTracking();
        
        return await query.ToListAsync(cancellationToken);
    }
}