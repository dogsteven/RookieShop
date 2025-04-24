using Microsoft.AspNetCore.Mvc;

namespace RookieShop.FrontStore.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public Exception Exception { get; set; } = null!;
}