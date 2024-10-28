namespace KTRegistration.Core.Contracts.Auth;
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.PIN)
            .InclusiveBetween(100000, 999999)
            .WithMessage("PIN must be exactly 6 digits.");

        RuleFor(x => x.ICNumber)
            .NotEmpty().WithMessage("IC Number is required.")
            .Length(12).WithMessage("IC Number must be exactly 12 characters.")
            .Matches(@"^\d{12}$").WithMessage("IC Number must be numeric and exactly 12 digits.");
    }
}
