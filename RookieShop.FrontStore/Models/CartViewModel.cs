using RookieShop.Shopping.ViewModels;

namespace RookieShop.FrontStore.Models;

public class CartViewModel
{
    public CartDto Cart { get; set; } = null!;
    public string? ContinueUrl { get; set; }
}