using Microsoft.EntityFrameworkCore;
using RookieShop.Application.Abstractions;
using RookieShop.Application.Entities;
using RookieShop.Application.Exceptions;
using RookieShop.Application.Models;

namespace RookieShop.Application.Services;

public class CategoryService
{
    private readonly RookieShopDbContext _dbContext;

    public CategoryService(RookieShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
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

    public async Task<int> CreateCategoryAsync(string name, string description, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = name,
            Description = description,
        };
        
        await _dbContext.Categories.AddAsync(category, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return category.Id;
    }

    public async Task UpdateCategoryAsync(int id, string name, string description, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException(id);
        }
        
        category.Name = name;
        category.Description = description;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException(id);
        }
        
        _dbContext.Categories.Remove(category);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}