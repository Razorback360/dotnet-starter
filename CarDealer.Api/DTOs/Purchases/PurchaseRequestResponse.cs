namespace CarDealer.Api.DTOs.Purchases;

public class PurchaseRequestResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
