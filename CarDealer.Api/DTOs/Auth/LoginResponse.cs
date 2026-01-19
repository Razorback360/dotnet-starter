namespace CarDealer.Api.DTOs.Auth;

public class LoginResponse
{
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Purpose { get; set; } = string.Empty;
}
