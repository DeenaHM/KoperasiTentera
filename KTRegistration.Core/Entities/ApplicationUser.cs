namespace KTRegistration.Core.Entities;
public sealed class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string DisplayPhoneNumber { get; set; } = string.Empty;
    public bool HasAgreedToTerms { get; set; }
    public bool IsBiometricEnabled { get; set; }
    public bool IsMigrated { get; set; }

    public ICollection<VerificationCode> VerificationCodes { get; set; } = new List<VerificationCode>();

}
