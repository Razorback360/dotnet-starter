using FluentValidation;

namespace CarDealer.Api.DTOs.Vehicles;

public class UpdateVehicleRequestValidator : AbstractValidator<UpdateVehicleRequest>
{
    public UpdateVehicleRequestValidator()
    {
        RuleFor(x => x.Make)
            .MaximumLength(100).WithMessage("Make must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Make));

        RuleFor(x => x.Model)
            .MaximumLength(100).WithMessage("Model must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Model));

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 2100).WithMessage("Year must be between 1900 and 2100")
            .When(x => x.Year.HasValue);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage must be greater than or equal to 0")
            .When(x => x.Mileage.HasValue);

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("Color must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.Status)
            .MaximumLength(50).WithMessage("Status must not exceed 50 characters")
            .Must(status => new[] { "Available", "Sold", "Pending" }.Contains(status))
            .WithMessage("Status must be one of: Available, Sold, Pending")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
}
