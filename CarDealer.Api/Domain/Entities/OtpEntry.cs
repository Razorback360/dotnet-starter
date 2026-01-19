namespace CarDealer.Api.Domain.Entities;

public class OtpEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConsumedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
