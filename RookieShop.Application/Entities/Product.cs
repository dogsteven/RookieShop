namespace RookieShop.Application.Entities;

public class Product
{
    public string Sku { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public decimal Price { get; set; }
    
    public Category Category { get; set; }
    
    public string ImageUrl { get; set; }
    
    public bool IsFeatured { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime UpdatedDate { get; set; }
    
#pragma warning disable CS8618, CS9264
    // Default constructor for Entity Framework Core 
    public Product() {}
#pragma warning restore CS8618, CS9264
}