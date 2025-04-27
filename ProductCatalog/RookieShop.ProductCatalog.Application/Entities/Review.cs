namespace RookieShop.ProductCatalog.Application.Entities;

public class Review
{
    public Guid WriterId { get; set; }
    
    public string ProductSku { get; set; }
    
    public string WriterName { get; set; }
    
    public int Score { get; set; }
    
    public string Comment { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public ICollection<ReviewReaction> Reactions { get; set; }
        
#pragma warning disable CS8618, CS9264
    public Review() {}
#pragma warning restore CS8618, CS9264
}

public class ReviewReaction
{
    public Guid ReactorId { get; set; }
    
    public Guid WriterId { get; set; }
    
    public string ProductSku { get; set; }
    
    public ReviewReactionType Type { get; set; }
    
#pragma warning disable CS8618, CS9264
    public ReviewReaction() {}
#pragma warning restore CS8618, CS9264
}

public enum ReviewReactionType
{
    Like, Dislike
}