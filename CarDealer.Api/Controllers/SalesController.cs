using CarDealer.Api.Data;
using CarDealer.Api.Domain.Entities;
using CarDealer.Api.DTOs.Auth;
using CarDealer.Api.DTOs.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.Api.Controllers;

[ApiController]
[Route("sales")]
[Authorize(Policy = "AdminOnly")]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<SalesController> _logger;

    public SalesController(AppDbContext context, ILogger<SalesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("{requestId}/process")]
    [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaleResponse>> ProcessSale(int requestId)
    {
        // Find purchase request
        var purchaseRequest = await _context.PurchaseRequests
            .Include(pr => pr.Vehicle)
            .Include(pr => pr.User)
            .FirstOrDefaultAsync(pr => pr.Id == requestId);

        if (purchaseRequest == null)
        {
            return NotFound(new ErrorResponse { Message = "Purchase request not found" });
        }

        // Check if request is pending
        if (purchaseRequest.Status != "Pending")
        {
            return BadRequest(new ErrorResponse { Message = $"Purchase request is not pending. Current status: {purchaseRequest.Status}" });
        }

        // Check if vehicle is available
        if (purchaseRequest.Vehicle.Status != "Available")
        {
            return BadRequest(new ErrorResponse { Message = "Vehicle is not available" });
        }

        // Create sale record
        var sale = new Sale
        {
            UserId = purchaseRequest.UserId,
            VehicleId = purchaseRequest.VehicleId,
            SoldAt = DateTime.UtcNow,
            Price = purchaseRequest.Vehicle.Price
        };

        _context.Sales.Add(sale);

        // Update purchase request status
        purchaseRequest.Status = "Approved";

        // Update vehicle status
        purchaseRequest.Vehicle.Status = "Sold";

        await _context.SaveChangesAsync();

        _logger.LogInformation("Sale processed - SaleId: {SaleId}, VehicleId: {VehicleId}, UserId: {UserId}, Price: {Price}", 
            sale.Id, sale.VehicleId, sale.UserId, sale.Price);

        return Ok(new SaleResponse
        {
            Message = "Sale processed successfully",
            Sale = new SaleInfo
            {
                Id = sale.Id,
                UserId = sale.UserId,
                VehicleId = sale.VehicleId,
                SoldAt = sale.SoldAt,
                Price = sale.Price
            }
        });
    }
}
