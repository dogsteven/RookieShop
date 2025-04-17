namespace RookieShop.ProductCatalog.Application.Models;

public class Pagination<TItem>
{
    public long Count { get; init; }
    
    public int PageNumber { get; init; }
    
    public int PageSize { get; init; }
    
    public IEnumerable<TItem> Items { get; init; } = [];
}