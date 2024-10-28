namespace KTRegistration.Core.Contracts.Auth;
public record AuthResponse(string Fullname ,string Token, string ICNumber, int ExpiresIn,  string RefreshToken,int RefreshTokenExpiresIn);

