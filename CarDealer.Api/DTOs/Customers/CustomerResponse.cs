namespace CarDealer.Api.DTOs.Customers;

public class CustomerResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
