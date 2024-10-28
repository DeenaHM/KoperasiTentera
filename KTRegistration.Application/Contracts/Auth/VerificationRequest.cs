namespace KTRegistration.Core.Contracts.Auth;
public record VerificationRequest(string ICNumber, VerificationPurpose Purpose);