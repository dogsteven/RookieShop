namespace RookieShop.Customers;

public class Customer
{
    public Guid Id { get; set; }
    
    public string Username { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public bool EmailVerified { get; set; }
    
    public bool Enabled { get; set; }
}