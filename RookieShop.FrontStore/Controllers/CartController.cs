using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Middlewares;
using RookieShop.FrontStore.Models;
using RookieShop.FrontStore.Modules.Shopping.Abstractions;

namespace RookieShop.FrontStore.Controllers;

[TypeFilter(typeof(QueryCartActionFilter))]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize(Roles = "customer")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var cart = await _cartService.GetCartAsync(cancellationToken);
        
        return View(new CartViewModel
        {
            Cart = cart
        });
    }

    public class AddItemToCartForm
    {
        [Required, MinLength(1), MaxLength(16)]
        public string Sku { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
#pragma warning disable CS8618, CS9264
        public AddItemToCartForm() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> AddItemToCart([FromForm] AddItemToCartForm form, string? redirectUrl,
        CancellationToken cancellationToken)
    {
        await _cartService.AddItemToCartAsync(form.Sku, form.Quantity, cancellationToken);

        if (redirectUrl != null)
        {
            return Redirect(redirectUrl);
        }
        
        return RedirectToAction("Index", "Cart");
    }

    public class AdjustItemQuantityForm
    {
        [Required, MinLength(1)]
        public IEnumerable<Adjustment> Adjustments { get; set; }
        
        public class Adjustment
        {
            [Required, MinLength(1), MaxLength(16)]
            public string Sku { get; set; }
            
            [Required, Range(0, int.MaxValue)]
            public int NewQuantity { get; set; }
            
#pragma warning disable CS8618, CS9264
            public Adjustment() {}
#pragma warning restore CS8618, CS9264
        }
        
#pragma warning disable CS8618, CS9264
        public AdjustItemQuantityForm() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> AdjustItemQuantity([FromForm] AdjustItemQuantityForm form, string? redirectUrl,
        CancellationToken cancellationToken)
    {
        await _cartService.AdjustItemQuantityAsync(form.Adjustments.Select(adjustment => new QuantityAdjustment(adjustment.Sku, adjustment.NewQuantity)), cancellationToken);

        if (redirectUrl != null)
        {
            return Redirect(redirectUrl);
        }
        
        return RedirectToAction("Index", "Cart");
    }

    public class RemoveItemFromCartForm
    {
        [Required, MinLength(1), MaxLength(16)]
        public string Sku { get; set; }
        
#pragma warning disable CS8618, CS9264
        public RemoveItemFromCartForm() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> RemoveItemFromCart([FromForm] RemoveItemFromCartForm form, string? redirectUrl,
        CancellationToken cancellationToken)
    {
        await _cartService.RemoveItemFromCartAsync(form.Sku, cancellationToken);

        if (redirectUrl != null)
        {
            return Redirect(redirectUrl);
        }
        
        return RedirectToAction("Index", "Cart");
    }
}