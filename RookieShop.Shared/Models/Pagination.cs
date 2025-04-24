namespace RookieShop.Shared.Models;

public class Pagination<TItem>
{
    public long Count { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }

    public IEnumerable<TItem> Items { get; set; } = null!;
}

public static class PaginationMappingExtensions
{
    public static Pagination<TTarget> Map<TItem, TTarget>(this Pagination<TItem> pagination,
        Func<TItem, TTarget> mapper)
    {
        return new Pagination<TTarget>
        {
            Count = pagination.Count,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            Items = pagination.Items.Select(mapper)
        };
    }
}