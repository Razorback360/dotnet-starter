namespace CarDealer.Api.Domain.Entities;

public class PurchaseRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}
