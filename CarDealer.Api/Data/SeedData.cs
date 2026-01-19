using System.Security.Cryptography;
using System.Text;
using CarDealer.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Check if data already exists
        if (await context.Users.AnyAsync() || await context.Vehicles.AnyAsync())
        {
            return; // Database already seeded
        }

        // Create admin user
        var adminUser = new User
        {
            Email = "admin@cardealer.com",
            PasswordHash = HashPassword("Admin123!"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);

        // Create 10 vehicles
        var vehicles = new List<Vehicle>
        {
            new Vehicle
            {
                Make = "Toyota",
                Model = "Camry",
                Year = 2023,
                Price = 28500.00m,
                Mileage = 15000,
                Color = "Silver",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "Honda",
                Model = "Accord",
                Year = 2022,
                Price = 26800.00m,
                Mileage = 22000,
                Color = "Black",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "Ford",
                Model = "F-150",
                Year = 2024,
                Price = 45000.00m,
                Mileage = 5000,
                Color = "Blue",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "Chevrolet",
                Model = "Silverado",
                Year = 2023,
                Price = 42000.00m,
                Mileage = 8000,
                Color = "White",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "BMW",
                Model = "X5",
                Year = 2023,
                Price = 65000.00m,
                Mileage = 12000,
                Color = "Gray",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "Mercedes-Benz",
                Model = "C-Class",
                Year = 2022,
                Price = 48000.00m,
                Mileage = 18000,
                Color = "Black",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "Audi",
                Model = "A4",
                Year = 2023,
                Price = 44000.00m,
                Mileage = 10000,
                Color = "Red",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "Tesla",
                Model = "Model 3",
                Year = 2024,
                Price = 52000.00m,
                Mileage = 3000,
                Color = "White",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "Nissan",
                Model = "Altima",
                Year = 2022,
                Price = 24500.00m,
                Mileage = 25000,
                Color = "Silver",
                Status = "Available"
            },
            new Vehicle
            {
                Make = "Mazda",
                Model = "CX-5",
                Year = 2023,
                Price = 32000.00m,
                Mileage = 14000,
                Color = "Blue",
                Status = "Available"
            }
        };

        context.Vehicles.AddRange(vehicles);
        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
