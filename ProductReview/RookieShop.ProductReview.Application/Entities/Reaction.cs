namespace RookieShop.ProductReview.Application.Entities;

public class Reaction
{
    public Guid ReactorId { get; set; }
    
    public Guid WriterId { get; set; }
    
    public string ProductSku { get; set; }
    
    public ReactionType Type { get; set; }
    
#pragma warning disable CS8618, CS9264
    public Reaction() {}
#pragma warning restore CS8618, CS9264
}

public enum ReactionType
{
    Like, Dislike
}