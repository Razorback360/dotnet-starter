namespace CarDealer.Api.DTOs.Vehicles;

public class VehicleUpdateInitiatedResponse
{
    public string Message { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public string Purpose { get; set; } = string.Empty;
}
