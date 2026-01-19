using FluentValidation;

namespace CarDealer.Api.DTOs.Auth;

public class OtpVerifyRequestValidator : AbstractValidator<OtpVerifyRequest>
{
    public OtpVerifyRequestValidator()
    {
        RuleFor(x => x.Purpose)
            .NotEmpty().WithMessage("Purpose is required")
            .MaximumLength(100).WithMessage("Purpose must not exceed 100 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(6).WithMessage("Code must be exactly 6 digits")
            .Matches("^[0-9]+$").WithMessage("Code must contain only digits");

        RuleFor(x => x.UserId)
            .NotNull().WithMessage("UserId is required")
            .GreaterThan(0).WithMessage("UserId must be greater than 0");
    }
}
