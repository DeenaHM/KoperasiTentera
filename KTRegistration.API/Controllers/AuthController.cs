namespace KTRegistration.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return response.IsFailure ? response.ToProblem() : Ok(response.Data);
    }

    [HttpPatch("agree-to-terms/{ICNumber}")]
    public async Task<IActionResult> AgreeToTermsAndPrivacy(string ICNumber, CancellationToken cancellationToken)
    {
        var response = await _authService.AgreeToTermsAndPrivacyAsync(ICNumber);
        return response.IsFailure ? response.ToProblem() : Ok(response.Data);
    }

    [HttpPost("set-pin")]
    public async Task<IActionResult> SetPinAsync([FromBody] SetPinRequest request)
    {
        var response = await _authService.SetUserPINAsync(request.ICNumber, request.PIN);
        return response.IsFailure ? response.ToProblem() : Ok(response.Data);
    }

    [HttpPatch("enable-biometric/{ICNumber}")]
    public async Task<IActionResult> EnableBiometricLogin(string ICNumber)
    {
        var response = await _authService.EnableBiometricLoginAsync(ICNumber);
        return response.IsFailure ? response.ToProblem() : Ok(response.Data);
    }

    [HttpPost("send-verification-code")]
    public async Task<IActionResult> SendVerificationCode([FromBody] VerificationRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.SendVerificationCodeAsync(request.ICNumber, request.Purpose, cancellationToken);
        return response.IsFailure ? response.ToProblem() : Ok(response.Data);
    }


    [HttpPost("confirm-verification-code")]
    public async Task<IActionResult> ConfirmVerificationCode([FromBody] ConfirmVerificationCodeRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.ConfirmVerificationCodeAsync(request.ICNumber, request.Code, request.Purpose, cancellationToken);
        return response.IsFailure ? response.ToProblem() : Ok(response.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return response.IsFailure ? response.ToProblem() : Ok(response.Data);
    }
}
