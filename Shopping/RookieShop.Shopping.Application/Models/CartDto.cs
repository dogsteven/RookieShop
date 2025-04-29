namespace RookieShop.Shopping.Application.Models;

public class CartDto
{
    public Guid Id { get; init; }

    public IEnumerable<CartItemDto> Items { get; init; } = null!;
}

public class CartItemDto
{
    public string Sku { get; init; } = null!;
    
    public string Name { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public Guid ImageId { get; init; }
    
    public int Quantity { get; init; }
}