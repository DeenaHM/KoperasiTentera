namespace KTRegistration.Core.Contracts.Auth;
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.ICNumber)
          .NotEmpty().WithMessage("IC Number is required.")
          .Length(12).WithMessage("IC Number must be exactly 12 characters.")
          .Matches(@"^\d{12}$").WithMessage("IC Number must be numeric and exactly 12 digits.");

        RuleFor(x => x.FullName)
          .NotEmpty().WithMessage("FullName is required.")
          .Length(10, 50).WithMessage("FullName must be between {MinLength} and {MaxLength} characters.")
          .Matches(@"^[a-zA-Z\s]+$").WithMessage("FullName can only contain letters and spaces.");

        RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required.")
          .Length(4, 50).WithMessage("Email must be between {MinLength} and {MaxLength} characters.")
          .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.DisplayPhoneNumber)
          .NotEmpty().WithMessage("Mobile number is required.")
          .Matches(@"^\+60\s\d{2}\s\d{3}\s\d{4}$").WithMessage("Mobile number must be in the format +60 XX XXX XXXX, with spaces.");
    }

}
