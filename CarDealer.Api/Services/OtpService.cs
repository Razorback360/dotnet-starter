using CarDealer.Api.Data;
using CarDealer.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CarDealer.Api.Services;

public class OtpService
{
    private readonly AppDbContext _context;
    private readonly ILogger<OtpService> _logger;
    private const int OtpExpirationMinutes = 5;
    private const int OtpLength = 6;

    public OtpService(AppDbContext context, ILogger<OtpService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GenerateOtpAsync(int userId, string purpose)
    {
        // Generate random 6-digit OTP
        var otp = GenerateRandomOtp();

        // Hash the OTP before storing
        var codeHash = HashOtp(otp);

        // Create OTP entry
        var otpEntry = new OtpEntry
        {
            UserId = userId,
            Purpose = purpose,
            CodeHash = codeHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpirationMinutes),
            ConsumedAt = null
        };

        _context.OtpEntries.Add(otpEntry);
        await _context.SaveChangesAsync();

        // Log OTP to console (simulated delivery)
        _logger.LogInformation(
            "OTP Generated - UserId: {UserId}, Purpose: {Purpose}, Code: {Code}, ExpiresAt: {ExpiresAt}",
            userId, purpose, otp, otpEntry.ExpiresAt);

        return otp;
    }

    public async Task<bool> VerifyOtpAsync(int userId, string purpose, string code)
    {
        var codeHash = HashOtp(code);

        // Find matching OTP entry
        var otpEntry = await _context.OtpEntries
            .Where(o => o.UserId == userId &&
                       o.Purpose == purpose &&
                       o.CodeHash == codeHash &&
                       o.ConsumedAt == null)
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefaultAsync();

        if (otpEntry == null)
        {
            _logger.LogWarning(
                "OTP Verification Failed - UserId: {UserId}, Purpose: {Purpose}, Reason: Not found or already consumed",
                userId, purpose);
            return false;
        }

        // Check if expired
        if (otpEntry.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning(
                "OTP Verification Failed - UserId: {UserId}, Purpose: {Purpose}, Reason: Expired",
                userId, purpose);
            return false;
        }

        _logger.LogInformation(
            "OTP Verified Successfully - UserId: {UserId}, Purpose: {Purpose}",
            userId, purpose);

        return true;
    }

    public async Task<bool> ConsumeOtpAsync(int userId, string purpose, string code)
    {
        var codeHash = HashOtp(code);

        // Find and consume the OTP
        var otpEntry = await _context.OtpEntries
            .Where(o => o.UserId == userId &&
                       o.Purpose == purpose &&
                       o.CodeHash == codeHash &&
                       o.ConsumedAt == null)
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefaultAsync();

        if (otpEntry == null)
        {
            _logger.LogWarning(
                "OTP Consumption Failed - UserId: {UserId}, Purpose: {Purpose}, Reason: Not found or already consumed",
                userId, purpose);
            return false;
        }

        // Check if expired
        if (otpEntry.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning(
                "OTP Consumption Failed - UserId: {UserId}, Purpose: {Purpose}, Reason: Expired",
                userId, purpose);
            return false;
        }

        // Mark as consumed
        otpEntry.ConsumedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "OTP Consumed - UserId: {UserId}, Purpose: {Purpose}, ConsumedAt: {ConsumedAt}",
            userId, purpose, otpEntry.ConsumedAt);

        return true;
    }

    private string GenerateRandomOtp()
    {
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();
        return otp;
    }

    private string HashOtp(string code)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(code);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

public static class OtpPurpose
{
    public const string Register = "Register";
    public const string Login = "Login";
    public const string Purchase = "Purchase";
    public const string UpdateVehicle = "UpdateVehicle";
}
