namespace CarDealer.Api.DTOs.Auth;

public class RegisterResponse
{
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Purpose { get; set; } = string.Empty;
}
