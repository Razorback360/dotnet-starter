using CarDealer.Api.Data;
using CarDealer.Api.Domain.Entities;
using CarDealer.Api.DTOs.Auth;
using CarDealer.Api.DTOs.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarDealer.Api.Controllers;

[ApiController]
[Route("vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(AppDbContext context, ILogger<VehiclesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<VehicleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VehicleResponse>>> GetVehicles(
        [FromQuery] string? make,
        [FromQuery] string? model,
        [FromQuery] int? minYear,
        [FromQuery] int? maxYear,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? status)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(make))
        {
            query = query.Where(v => v.Make.Contains(make));
        }

        if (!string.IsNullOrEmpty(model))
        {
            query = query.Where(v => v.Model.Contains(model));
        }

        if (minYear.HasValue)
        {
            query = query.Where(v => v.Year >= minYear.Value);
        }

        if (maxYear.HasValue)
        {
            query = query.Where(v => v.Year <= maxYear.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(v => v.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(v => v.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(v => v.Status == status);
        }

        var vehicles = await query.Select(v => new VehicleResponse
        {
            Id = v.Id,
            Make = v.Make,
            Model = v.Model,
            Year = v.Year,
            Price = v.Price,
            Mileage = v.Mileage,
            Color = v.Color,
            Status = v.Status
        }).ToListAsync();

        return Ok(vehicles);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponse>> GetVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound(new ErrorResponse { Message = "Vehicle not found" });
        }

        return Ok(new VehicleResponse
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Price = vehicle.Price,
            Mileage = vehicle.Mileage,
            Color = vehicle.Color,
            Status = vehicle.Status
        });
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleResponse>> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var vehicle = new Vehicle
        {
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Price = request.Price,
            Mileage = request.Mileage,
            Color = request.Color,
            Status = request.Status
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Vehicle created - Id: {VehicleId}, Make: {Make}, Model: {Model}", 
            vehicle.Id, vehicle.Make, vehicle.Model);

        var response = new VehicleResponse
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Price = vehicle.Price,
            Mileage = vehicle.Mileage,
            Color = vehicle.Color,
            Status = vehicle.Status
        };

        return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponse>> UpdateVehicle(int id, [FromBody] UpdateVehicleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound(new ErrorResponse { Message = "Vehicle not found" });
        }

        // Apply updates (only update fields that are provided)
        if (!string.IsNullOrEmpty(request.Make))
            vehicle.Make = request.Make;
        
        if (!string.IsNullOrEmpty(request.Model))
            vehicle.Model = request.Model;
        
        if (request.Year.HasValue)
            vehicle.Year = request.Year.Value;
        
        if (request.Price.HasValue)
            vehicle.Price = request.Price.Value;
        
        if (request.Mileage.HasValue)
            vehicle.Mileage = request.Mileage.Value;
        
        if (!string.IsNullOrEmpty(request.Color))
            vehicle.Color = request.Color;
        
        if (!string.IsNullOrEmpty(request.Status))
            vehicle.Status = request.Status;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Vehicle updated - Id: {VehicleId}, Make: {Make}, Model: {Model}", 
            vehicle.Id, vehicle.Make, vehicle.Model);

        return Ok(new VehicleResponse
        {
            Id = vehicle.Id,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Price = vehicle.Price,
            Mileage = vehicle.Mileage,
            Color = vehicle.Color,
            Status = vehicle.Status
        });
    }
}
