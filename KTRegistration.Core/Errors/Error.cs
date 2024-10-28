namespace KTRegistration.Core.Errors;
public record Error
{
    public string Code { get; init; }
    public string Description { get; init; }
    public int StatusCode { get; init; }

    // Constructor لتقبل 3 معاملات
    public Error(string code, string description, int statusCode)
    {
        Code = code;
        Description = description;
        StatusCode = statusCode;
    }

    public static readonly Error None = new Error(string.Empty, string.Empty, StatusCodes.Status200OK);
}