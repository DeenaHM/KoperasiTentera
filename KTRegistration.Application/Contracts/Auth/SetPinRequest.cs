namespace KTRegistration.Core.Contracts.Auth;
public record SetPinRequest(string ICNumber, int PIN, int ConfirmedPIN);
