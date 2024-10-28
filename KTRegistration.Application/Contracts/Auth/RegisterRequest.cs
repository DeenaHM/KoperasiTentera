namespace KTRegistration.Core.Contracts.Auth;
public record RegisterRequest(
    string Email,
    string DisplayPhoneNumber,
    string FullName,
    string ICNumber
    )
{
    // Property to clean the DisplayMobileNumber and return the MobileNumber
    public string PhoneNumber =>
        string.IsNullOrEmpty(DisplayPhoneNumber) ? "" : Regex.Replace(DisplayPhoneNumber, @"[^\d]", "");
}