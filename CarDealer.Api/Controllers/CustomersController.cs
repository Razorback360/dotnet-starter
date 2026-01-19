using CarDealer.Api.DTOs.Customers;
using CarDealer.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.Api.Controllers;

[ApiController]
[Route("customers")]
[Authorize(Policy = "AdminOnly")]
public class CustomersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(UserService userService, ILogger<CustomersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerResponse>>> GetCustomers()
    {
        var customers = await _userService.GetAllCustomersAsync();

        var result = customers.Select(c => new CustomerResponse
        {
            Id = c.Id,
            Email = c.Email,
            Role = c.Role,
            CreatedAt = c.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} customers", customers.Count);

        return Ok(result);
    }
}
