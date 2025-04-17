namespace RookieShop.ProductCatalog.Application.Entities;

public class Category
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
#pragma warning disable CS8618, CS9264
    public Category() {}
#pragma warning restore CS8618, CS9264
}