using System.Security.Cryptography;
using System.Text;
using CarDealer.Api.Data;
using CarDealer.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.Api.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateUserAsync(string email, string password, string role = "Customer")
    {
        var passwordHash = HashPassword(password);

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User Created - Email: {Email}, Role: {Role}", email, role);

        return user;
    }

    public bool VerifyPassword(User user, string password)
    {
        var passwordHash = HashPassword(password);
        return user.PasswordHash == passwordHash;
    }

    public async Task<List<User>> GetAllCustomersAsync()
    {
        return await _context.Users
            .Where(u => u.Role == "Customer")
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
