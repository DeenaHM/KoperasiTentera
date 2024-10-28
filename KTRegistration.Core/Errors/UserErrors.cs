namespace KTRegistration.Core.Errors;
public static class UserErrors
{
    // Errors related to Registration
    public static readonly Error UserCreationFailed =
        new("User.CreationFailed", "User Creation Failed , process not completed", StatusCodes.Status400BadRequest);

    public static readonly Error UserDuplicated =
        new("User.Duplicated", "User already Exists", StatusCodes.Status409Conflict);

    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);

    public static readonly Error FailedToUpdatePIN =
        new("User.FailedToUpdatePIN", "Failed to update PIN", StatusCodes.Status500InternalServerError);

    public static readonly Error UserAlreadyMigrated =
    new("User.AlreadyMigrated", "User has already been migrated. Please log in using your PIN.", StatusCodes.Status409Conflict);

    public static readonly Error UserUpdateFailed =
    new("User.UpdateFailed", "Failed to update user information.", StatusCodes.Status500InternalServerError);

    public static readonly Error EmailOrPhoneNotConfirmed =
    new("User.EmailOrPhoneNotConfirmed", "User 'email' or 'phone' number not confirmed", StatusCodes.Status403Forbidden);

    // Errors related to Terms & Privacy Policy
    public static readonly Error TermsNotAccepted =
        new("User.TermsNotAccepted", "User has not accepted the Terms and Privacy Policy", StatusCodes.Status400BadRequest);

    public static readonly Error FailedToUpdateAgreement =
        new("User.FailedToUpdateAgreement", "Failed to update the agreement status", StatusCodes.Status500InternalServerError);

    public static readonly Error FailedToUpdateBiometricStatus =
    new("User.FailedToUpdateBiometricStatus", "Failed to update biometric login status", StatusCodes.Status500InternalServerError);

    // Errors related to Login
    public static readonly Error UserNotFound =
        new("User.NotFound", "User Id not found", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidPIN =
        new("User.InvalidPIN", "Invalid PIN provided", StatusCodes.Status401Unauthorized);


}
