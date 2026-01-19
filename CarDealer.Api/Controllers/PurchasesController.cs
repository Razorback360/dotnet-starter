using CarDealer.Api.Data;
using CarDealer.Api.Domain.Entities;
using CarDealer.Api.DTOs.Auth;
using CarDealer.Api.DTOs.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarDealer.Api.Controllers;

[ApiController]
[Route("purchases")]
[Authorize(Policy = "CustomerOnly")]
public class PurchasesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<PurchasesController> _logger;

    public PurchasesController(AppDbContext context, ILogger<PurchasesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("request")]
    [ProducesResponseType(typeof(PurchaseRequestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PurchaseRequestResponse>> CreatePurchaseRequest([FromBody] PurchaseRequestCreate request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get userId from claims
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user" });
        }

        // Check if vehicle exists
        var vehicle = await _context.Vehicles.FindAsync(request.VehicleId);
        if (vehicle == null)
        {
            return NotFound(new ErrorResponse { Message = "Vehicle not found" });
        }

        // Check if vehicle is available
        if (vehicle.Status != "Available")
        {
            return BadRequest(new ErrorResponse { Message = "Vehicle is not available for purchase" });
        }

        // Create purchase request
        var purchaseRequest = new PurchaseRequest
        {
            UserId = userId,
            VehicleId = request.VehicleId,
            RequestedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        _context.PurchaseRequests.Add(purchaseRequest);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Purchase request created - Id: {Id}, UserId: {UserId}, VehicleId: {VehicleId}", 
            purchaseRequest.Id, userId, request.VehicleId);

        return CreatedAtAction(nameof(GetPurchaseHistory), new PurchaseRequestResponse
        {
            Id = purchaseRequest.Id,
            UserId = purchaseRequest.UserId,
            VehicleId = purchaseRequest.VehicleId,
            RequestedAt = purchaseRequest.RequestedAt,
            Status = purchaseRequest.Status,
            Message = "Purchase request created successfully. Awaiting admin approval."
        });
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(List<PurchaseHistoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PurchaseHistoryItem>>> GetPurchaseHistory()
    {
        // Get userId from claims
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user" });
        }

        var purchaseRequests = await _context.PurchaseRequests
            .Include(pr => pr.Vehicle)
            .Where(pr => pr.UserId == userId)
            .OrderByDescending(pr => pr.RequestedAt)
            .ToListAsync();

        var history = purchaseRequests.Select(pr => new PurchaseHistoryItem
        {
            Id = pr.Id,
            VehicleId = pr.VehicleId,
            VehicleMake = pr.Vehicle.Make,
            VehicleModel = pr.Vehicle.Model,
            VehicleYear = pr.Vehicle.Year,
            VehiclePrice = pr.Vehicle.Price,
            RequestedAt = pr.RequestedAt,
            Status = pr.Status
        }).ToList();

        _logger.LogInformation("Retrieved purchase history for UserId: {UserId}, Count: {Count}", 
            userId, history.Count);

        return Ok(history);
    }
}
