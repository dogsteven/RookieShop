namespace RookieShop.Customers;

public interface ICustomerService
{
    public Task<IEnumerable<Customer>> GetCustomersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}