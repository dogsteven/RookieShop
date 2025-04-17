namespace RookieShop.ProductReview.Application.Entities;

public class Review
{
    public Guid WriterId { get; set; }
    
    public string ProductSku { get; set; }
    
    public int Score { get; set; }
    
    public string Comment { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public ICollection<Reaction> Reactions { get; set; }
        
#pragma warning disable CS8618, CS9264
    public Review() {}
#pragma warning restore CS8618, CS9264
}