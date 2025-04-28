namespace RookieShop.FrontStore.Models.Shared.Components;

public class PaginationViewModel
{
    public long Count { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Dictionary<string, string> OtherParams { get; set; } = null!;
}