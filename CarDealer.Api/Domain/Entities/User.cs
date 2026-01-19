namespace CarDealer.Api.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<OtpEntry> OtpEntries { get; set; } = new List<OtpEntry>();
}
