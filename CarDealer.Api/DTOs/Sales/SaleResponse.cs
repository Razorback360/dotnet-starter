namespace CarDealer.Api.DTOs.Sales;

public class SaleResponse
{
    public string Message { get; set; } = string.Empty;
    public SaleInfo Sale { get; set; } = null!;
}

public class SaleInfo
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public DateTime SoldAt { get; set; }
    public decimal Price { get; set; }
}
