using System.ComponentModel.DataAnnotations;

namespace CarDealer.Api.DTOs.Vehicles;

public class UpdateVehicleRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }

    [Range(1900, 2100)]
    public int? Year { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Price { get; set; }

    [Range(0, int.MaxValue)]
    public int? Mileage { get; set; }

    public string? Color { get; set; }
    public string? Status { get; set; }
}
