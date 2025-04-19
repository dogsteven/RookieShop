namespace RookieShop.ProductCatalog.Application.Entities;

public class Review
{
    public Guid WriterId { get; set; }
    
    public string ProductSku { get; set; }
    
    public string WriterName { get; set; }
    
    public int Score { get; set; }
    
    public string Comment { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public ICollection<Reaction> Reactions { get; set; }
        
#pragma warning disable CS8618, CS9264
    public Review() {}
#pragma warning restore CS8618, CS9264
}

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