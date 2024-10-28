namespace KTRegistration.Core.Contracts.Auth;
public class VerificationRequestValidator : AbstractValidator<VerificationRequest>
{
    public VerificationRequestValidator()
    {
        RuleFor(x => x.ICNumber)
          .NotEmpty().WithMessage("IC Number is required.")
          .Length(12).WithMessage("IC Number must be exactly 12 characters.")
          .Matches(@"^\d{12}$").WithMessage("IC Number must be numeric and exactly 12 digits.");

        RuleFor(x => x.Purpose)
            .IsInEnum().WithMessage("Purpose must be either 'Email' or 'SMS'.");
    }
}
