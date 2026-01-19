using System.ComponentModel.DataAnnotations;

namespace CarDealer.Api.DTOs.Vehicles;

public class CreateVehicleRequest
{
    [Required]
    public string Make { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }

    [Required]
    public string Color { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;
}
