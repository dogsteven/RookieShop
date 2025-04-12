namespace RookieShop.Application.Abstractions;

public interface IPurchaseChecker
{
    public ValueTask<bool> CheckIfCustomerHasPurchasedProductAsync(Guid customerId, string sku, CancellationToken cancellationToken);
}