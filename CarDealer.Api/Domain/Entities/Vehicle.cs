namespace CarDealer.Api.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public int Mileage { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
