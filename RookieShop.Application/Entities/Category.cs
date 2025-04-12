namespace RookieShop.Application.Entities;

public class Category
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
#pragma warning disable CS8618, CS9264
    // Default constructor for Entity Framework Core
    public Category() {}
#pragma warning restore CS8618, CS9264
}