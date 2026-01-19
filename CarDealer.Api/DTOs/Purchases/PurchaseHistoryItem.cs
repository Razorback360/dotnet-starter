namespace CarDealer.Api.DTOs.Purchases;

public class PurchaseHistoryItem
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string VehicleMake { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public int VehicleYear { get; set; }
    public decimal VehiclePrice { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
