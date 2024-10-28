public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly ApplicationDbContext _dbContext;

    public AuthService(UserManager<ApplicationUser> userManager, ILogger<AuthService> logger, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<Result<Response>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Check if the user exists by ICNumber
        var user = await _userManager.FindByIdAsync(request.ICNumber);

        // If user already exists
        if (user != null)
        {
            // Check if the user is already migrated
            if (user.IsMigrated)
            {
                _logger.LogWarning("User with ICNumber {0} has already been migrated.", request.ICNumber);
                return Result.Failure<Response>(UserErrors.UserAlreadyMigrated);
            }
            else
            {
                // Update the user's info from the registration request
                return await UpdateAndMigrateUserAsync(user, request, cancellationToken);
            }
        }

        // If the user does not exist, proceed with regular registration
        _logger.LogInformation("User with ICNumber {0} does not exist. Proceeding with registration.", request.ICNumber);
        var newUser = request.Adapt<ApplicationUser>();

        // Create the new user
        var creationResult = await _userManager.CreateAsync(newUser);
        if (!creationResult.Succeeded)
        {
            _logger.LogError("User creation failed for {0} - with ICNumber {1}", newUser.Email, newUser.Id);
            return Result.Failure<Response>(UserErrors.UserCreationFailed);
        }

        // Send SMS verification code
        await GenerateAndSendVerificationCodeAsync(newUser, VerificationPurpose.SMS, cancellationToken);
        return Result.Success<Response>(new Response("Verification code sent via SMS."));
    }
    private async Task<Result<Response>> UpdateAndMigrateUserAsync(ApplicationUser user, RegisterRequest request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("User with ICNumber {0} exists but is not migrated. Proceeding with updating the user's information.", request.ICNumber);

        // Update user's information
        user.UserName = request.ICNumber;
        user.Email = request.Email.ToLower();
        user.FullName = request.FullName;
        user.DisplayPhoneNumber = request.DisplayPhoneNumber;
        user.PhoneNumber = request.PhoneNumber;

        // Update the user in the database
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            _logger.LogError("Failed to update user {0} - with ICNumber {1}", user.Email, user.Id);
            return Result.Failure<Response>(UserErrors.UserUpdateFailed);
        }

        // Continue migration process
        return await MigrateUserAsync(user, cancellationToken);
    }
    private async Task<Result<Response>> MigrateUserAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting migration process for user {0}.", user.Id);

        await GenerateAndSendVerificationCodeAsync(user, VerificationPurpose.SMS, cancellationToken);

        _logger.LogInformation("User {0} is in the migration flow. Verification code sent.", user.Id);

        return Result.Success<Response>(new Response("User is in migration flow. Verification code sent."));
    }
    public async Task<Result<Response>> GenerateAndSendVerificationCodeAsync(ApplicationUser user, VerificationPurpose purpose, CancellationToken cancellationToken)
    {
        // Always generate a new code
        int code = new Random().Next(1000, 9999);
        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = code,
            ExpiryDate = DateTime.UtcNow.AddMinutes(5), // Code validity for 2 minutes
            Purpose = purpose,
            IsUsed = false
        };

        // Add or update verification code in database
        var existingCode = await _dbContext.VerificationCodes
            .FirstOrDefaultAsync(vc => vc.UserId == user.Id && vc.Purpose == purpose, cancellationToken);

        if (existingCode != null)
        {
            // Update existing code
            existingCode.Code = code;
            existingCode.ExpiryDate = DateTime.UtcNow.AddMinutes(5);
            existingCode.IsUsed = false;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            // Add new code
            _dbContext.VerificationCodes.Add(verificationCode);
        }

        _logger.LogInformation("Generated and stored new verification code {0} for user {1}.", code, user.Email ?? user.PhoneNumber);

        // Save changes to database
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Send the code via SMS or Email
        await SendVerificationCodeByBackgroundJobAsync(user, purpose, code);
        return Result.Success<Response>(new Response("Verification code sent."));
    }
    private async Task SendVerificationCodeByBackgroundJobAsync(ApplicationUser user, VerificationPurpose purpose, int code)
    {
        switch (purpose)
        {
            case VerificationPurpose.Email:
                _logger.LogInformation("Sending email verification code to {0}.", user.Email);
                // Simulate sending email
                // send email in a background job using (Hangfire) for example
                break;

            case VerificationPurpose.SMS:
                _logger.LogInformation("Sending SMS verification code to {0}.", user.PhoneNumber);
                // Simulate sending SMS
                // Integrate with third-party library to send SMS in a background job using (Hangfire) for example

                break;

            default:
                _logger.LogWarning("Unknown verification purpose: {0}.", purpose);
                break;
        }
    }
    public async Task<Result<Response>> ConfirmVerificationCodeAsync(string ICNumber, int code, VerificationPurpose purpose, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(ICNumber);
        if (user is null)
        {
            _logger.LogError("User with ICNumber {0} not found.", ICNumber);
            return Result.Failure<Response>(UserErrors.UserNotFound);
        }

        // Check for an unused and valid code (not expired)
        var verificationCode = await _dbContext.VerificationCodes
            .FirstOrDefaultAsync(vc => vc.UserId == user.Id && vc.Code == code && vc.Purpose == purpose && !vc.IsUsed && vc.ExpiryDate > DateTime.UtcNow, cancellationToken);

        if (verificationCode == null)
        {
            _logger.LogWarning("Invalid or expired verification code for user {0}.", user.Id);
            return Result.Failure<Response>(UserErrors.InvalidCode);
        }

        // Mark the verification code as used (soft delete) to keep a record for security reasons.
        verificationCode.IsUsed = true;

        // Verification
        switch (purpose)
        {
            case VerificationPurpose.Email:
                user.EmailConfirmed = true;
                _logger.LogInformation("Email confirmed for user {0}.", user.UserName);
                break;

            case VerificationPurpose.SMS:
                user.PhoneNumberConfirmed = true;
                _logger.LogInformation("Phone number confirmed for user {0}.", user.UserName);
                break;

            default:
                _logger.LogWarning("Unknown verification purpose: {0} for user {1}.", purpose, user.Id);
                break;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Verification code for user {0} confirmed and marked as used.", user.Id);
        return Result.Success(new Response("Verification code confirmed."));
    }
    public async Task<Result<Response>> SendVerificationCodeAsync(string ICNumber, VerificationPurpose purpose, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(ICNumber);

        if (user is null)
        {
            _logger.LogError("User with ICNumber {0} not found.", ICNumber);
            return Result.Failure<Response>(UserErrors.UserNotFound);
        }

        await GenerateAndSendVerificationCodeAsync(user, purpose, cancellationToken);
        return Result.Success(new Response("Verification code sent."));
    }
    public async Task<Result<Response>> AgreeToTermsAndPrivacyAsync(string ICNumber)
    {
        var user = await _userManager.FindByIdAsync(ICNumber);
        if (user is null)
        {
            _logger.LogError("User is null");
            return Result.Failure<Response>(UserErrors.UserNotFound);
        }

        user.HasAgreedToTerms = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            _logger.LogError("Failed to update agreement for user {0}", user.Id);
            return Result.Failure<Response>(UserErrors.FailedToUpdateAgreement);
        }

        _logger.LogInformation("User {0} agreed to terms and privacy policy.", user.Id);
        return Result.Success(new Response("Successfully agreed to terms and privacy policy."));
    }
    public async Task<Result<Response>> SetUserPINAsync(string ICNumber, int pin)
    {
        var user = await _userManager.FindByIdAsync(ICNumber);
        if (user is null)
        {
            _logger.LogError("User not found with ICNumber: {0}", ICNumber);
            return Result.Failure<Response>(UserErrors.UserNotFound);
        }

        // Check if email and phone number are confirmed
        if (!user.EmailConfirmed || !user.PhoneNumberConfirmed)
        {
            _logger.LogError("Cannot set PIN. Email or phone number not confirmed for user with ICNumber: {0}", ICNumber);
            return Result.Failure<Response>(UserErrors.EmailOrPhoneNotConfirmed);
        }

        var hashedPIN = _userManager.PasswordHasher.HashPassword(user, pin.ToString());
        user.PasswordHash = hashedPIN;
        user.IsMigrated = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            _logger.LogError("Failed to update PIN for user with ICNumber: {0}", ICNumber);
            return Result.Failure<Response>(UserErrors.FailedToUpdatePIN);
        }

        _logger.LogInformation("Successfully set PIN for user with ICNumber: {0}.", ICNumber);
        return Result.Success(new Response("PIN successfully set."));
    }
    public async Task<Result<Response>> EnableBiometricLoginAsync(string ICNumber)
    {
        var user = await _userManager.FindByIdAsync(ICNumber);
        if (user is null)
        {
            _logger.LogError("User not found with Id: {0}", ICNumber);
            return Result.Failure<Response>(UserErrors.UserNotFound);
        }

        user.IsBiometricEnabled = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            _logger.LogError("Failed to update biometric login status for user {0}", ICNumber);
            return Result.Failure<Response>(UserErrors.FailedToUpdateBiometricStatus);
        }

        _logger.LogInformation("Successfully enabled biometric login for user {0}.", ICNumber);
        return Result.Success(new Response("Biometric login enabled."));
    }
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.ICNumber);
        if (user == null)
        {
            _logger.LogError("Login failed: User not found with ICNumber: {0}", request.ICNumber);
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);
        }

        if (!user.EmailConfirmed || !user.PhoneNumberConfirmed)
        {
            _logger.LogError("Login failed: Email or phone number not confirmed for user with ICNumber: {0}", request.ICNumber);
            return Result.Failure<AuthResponse>(UserErrors.EmailOrPhoneNotConfirmed);
        }

        var isPinValid = await _userManager.CheckPasswordAsync(user, request.PIN.ToString());
        if (!isPinValid)
        {
            _logger.LogError("Login failed: Incorrect PIN for user with ICNumber: {0}", request.ICNumber);
            return Result.Failure<AuthResponse>(UserErrors.InvalidPIN);
        }

        _logger.LogInformation("Login successful for user with ICNumber: {0}", request.ICNumber);

        AuthResponse authResponse = new(
            user.FullName,
            "SimulatedJWTToken",
            user.Id.ToString(),
            3600,
            "SimulatedRefreshToken",
            604800);

        return Result.Success(authResponse);
    }
}
