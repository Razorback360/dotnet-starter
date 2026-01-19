using System.ComponentModel.DataAnnotations;

namespace CarDealer.Api.DTOs.Auth;

public class OtpVerifyRequest
{
    [Required]
    public string Purpose { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    public int? UserId { get; set; }
}
