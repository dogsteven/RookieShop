using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Customers;

namespace RookieShop.WebApi.Customers.Controllers;

[ApiController]
[Route("/customers/api")]
[Produces("application/problem+json")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "admin")]
    public async Task<IEnumerable<Customer>> GetCustomersAsync(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        return await _customerService.GetCustomersAsync(pageNumber ?? 1, pageSize ?? 20, cancellationToken);
    }
}