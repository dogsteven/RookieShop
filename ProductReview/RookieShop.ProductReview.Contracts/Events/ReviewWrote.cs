namespace RookieShop.ProductReview.Contracts.Events;

public class ReviewWrote
{
    public Guid WriterId { get; set; }

    public string ProductSku { get; set; } = null!;
    
    public int Score { get; set; }
}