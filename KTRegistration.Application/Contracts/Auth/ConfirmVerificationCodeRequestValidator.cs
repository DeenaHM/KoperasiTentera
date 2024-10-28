namespace KTRegistration.Core.Contracts.Auth;
public class ConfirmVerificationCodeRequestValidator : AbstractValidator<ConfirmVerificationCodeRequest>
{
    public ConfirmVerificationCodeRequestValidator()
    {
        RuleFor(x => x.ICNumber)
            .NotEmpty().WithMessage("IC Number is required.")
            .Length(12).WithMessage("IC Number must be exactly 12 characters.")
            .Matches(@"^\d{12}$").WithMessage("IC Number must be numeric and exactly 12 digits.");

        RuleFor(x => x.Code)
            .InclusiveBetween(1000, 9999).WithMessage("Code must be a 4-digit number.");

        RuleFor(x => x.Purpose)
            .IsInEnum().WithMessage("Purpose must be either 'Email' or 'SMS'.");
    }
}
