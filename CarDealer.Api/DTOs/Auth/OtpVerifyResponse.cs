namespace CarDealer.Api.DTOs.Auth;

public class OtpVerifyResponse
{
    public string Token { get; set; } = string.Empty;
    public UserInfo User { get; set; } = null!;
}

public class UserInfo
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
