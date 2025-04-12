namespace RookieShop.Application.Entities;

public class Rating
{
    public Guid CustomerId { get; set; }
    
    public string Sku { get; set; }
    
    public float Score { get; set; }
    
    public string Comment { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
#pragma warning disable CS8618, CS9264
    // Default constructor for Entity Framework Core
    public Rating() {}
#pragma warning restore CS8618, CS9264
}