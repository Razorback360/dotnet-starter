using FluentValidation;

namespace CarDealer.Api.DTOs.Purchases;

public class PurchaseRequestCreateValidator : AbstractValidator<PurchaseRequestCreate>
{
    public PurchaseRequestCreateValidator()
    {
        RuleFor(x => x.VehicleId)
            .GreaterThan(0).WithMessage("VehicleId must be greater than 0");
    }
}
