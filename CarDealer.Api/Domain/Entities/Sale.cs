namespace CarDealer.Api.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public DateTime SoldAt { get; set; }
    public decimal Price { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}
