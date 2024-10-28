
namespace KTRegistration.Core.Services.Auth;
/// <summary>
/// Interface for authentication and user management services, including registration, verification, and login functionalities.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user or updates an existing user's information if they are not migrated.
    /// </summary>
    /// <param name="request">The registration request containing the user's details.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A result containing a success message or failure with an appropriate error.</returns>
    Task<Result<Response>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates and sends a verification code via SMS or email to the user.
    /// </summary>
    /// <param name="user">The user for whom the verification code is generated.</param>
    /// <param name="purpose">The purpose of the verification (e.g., email or SMS).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A result indicating whether the code was sent successfully.</returns>
    Task<Result<Response>> GenerateAndSendVerificationCodeAsync(ApplicationUser user, VerificationPurpose purpose, CancellationToken cancellationToken);

    /// <summary>
    /// Confirms a verification code for a user based on their ICNumber and purpose (e.g., email or SMS).
    /// </summary>
    /// <param name="ICNumber">The user's ICNumber.</param>
    /// <param name="code">The verification code entered by the user.</param>
    /// <param name="purpose">The purpose of the verification (email or SMS).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A result indicating success or failure of the verification process.</returns>
    Task<Result<Response>> ConfirmVerificationCodeAsync(string ICNumber, int code, VerificationPurpose purpose, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a new verification code to the user via SMS or email.
    /// </summary>
    /// <param name="ICNumber">The user's ICNumber.</param>
    /// <param name="purpose">The purpose of the verification (email or SMS).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A result indicating whether the code was sent successfully.</returns>
    Task<Result<Response>> SendVerificationCodeAsync(string ICNumber, VerificationPurpose purpose, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the user's record to indicate that they have agreed to the terms and privacy policy.
    /// </summary>
    /// <param name="ICNumber">The user's identification number (ICNumber) used to locate the user.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a success message if the operation succeeds, or an error message if it fails.</returns>
    Task<Result<Response>> AgreeToTermsAndPrivacyAsync(string ICNumber);
    /// <summary>
    /// Sets the user's PIN after confirming their email and phone number.
    /// </summary>
    /// <param name="ICNumber">The user's ICNumber.</param>
    /// <param name="pin">The new PIN to be set for the user.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A result indicating success or failure of setting the PIN.</returns>
    Task<Result<Response>> SetUserPINAsync(string ICNumber, int pin);

    /// <summary>
    /// Enables biometric login for the user.
    /// </summary>
    /// <param name="ICNumber">The user's ICNumber.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A result indicating success or failure of enabling biometric login.</returns>
    Task<Result<Response>> EnableBiometricLoginAsync(string ICNumber);

    /// <summary>
    /// Handles the login process for the user, including checking their PIN and whether their email and phone number are confirmed.
    /// </summary>
    /// <param name="request">The login request containing the ICNumber and PIN.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A result containing authentication response if successful, or failure with an appropriate error.</returns>
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
}
