namespace RookieShop.Shared.Models;

public class Pagination<TItem>
{
    public long Count { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }

    public IEnumerable<TItem> Items { get; set; } = null!;
}