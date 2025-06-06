namespace RookieShop.ProductCatalog.ViewModels;

public class ReviewDto
{
    public Guid WriterId { get; init; }

    public string ProductSku { get; init; } = null!;
    
    public string WriterName { get; init; } = null!;
    
    public int Score { get; init; }

    public string Comment { get; init; } = null!;
    
    public DateTime CreatedDate { get; init; }
    
    public int NumberOfLikes { get; init; }
    
    public int NumberOfDislikes { get; init; }
}