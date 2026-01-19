namespace CarDealer.Api.DTOs.Purchases;

public class PurchaseRequestInitiatedResponse
{
    public string Message { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public string Purpose { get; set; } = string.Empty;
}
