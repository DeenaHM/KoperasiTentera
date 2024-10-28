namespace KTRegistration.Core.Contracts.Auth;
public record ConfirmVerificationCodeRequest(string ICNumber, int Code, VerificationPurpose Purpose);
