namespace RookieShop.Application.Models;

public class RatingDto
{
    public Guid CustomerId { get; init; }

    public string Sku { get; init; } = null!;
    
    public float Score { get; init; }

    public string Comment { get; init; } = null!;
    
    public DateTime CreatedDate { get; init; }
}