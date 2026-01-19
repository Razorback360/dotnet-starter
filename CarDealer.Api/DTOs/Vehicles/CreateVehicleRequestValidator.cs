using FluentValidation;

namespace CarDealer.Api.DTOs.Vehicles;

public class CreateVehicleRequestValidator : AbstractValidator<CreateVehicleRequest>
{
    public CreateVehicleRequestValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty().WithMessage("Make is required")
            .MaximumLength(100).WithMessage("Make must not exceed 100 characters");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(100).WithMessage("Model must not exceed 100 characters");

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 2100).WithMessage("Year must be between 1900 and 2100");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage must be greater than or equal to 0");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required")
            .MaximumLength(50).WithMessage("Color must not exceed 50 characters");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .MaximumLength(50).WithMessage("Status must not exceed 50 characters")
            .Must(status => new[] { "Available", "Sold", "Pending" }.Contains(status))
            .WithMessage("Status must be one of: Available, Sold, Pending");
    }
}
