using CarDealer.Api.DTOs.Auth;
using CarDealer.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly OtpService _otpService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserService userService,
        OtpService otpService,
        JwtTokenService jwtTokenService,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _otpService = otpService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if user already exists
        var existingUser = await _userService.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Conflict(new ErrorResponse { Message = "User with this email already exists" });
        }

        // Create user with Customer role
        var user = await _userService.CreateUserAsync(request.Email, request.Password, "Customer");

        // Generate OTP for registration
        var otpCode = await _otpService.GenerateOtpAsync(user.Id, "register");

        _logger.LogInformation("Registration initiated for email: {Email}", request.Email);

        return Ok(new RegisterResponse
        {
            Message = "Registration initiated. Please verify OTP.",
            UserId = user.Id,
            Purpose = "register"
        });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Find user by email
        var user = await _userService.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid email or password" });
        }

        // Verify password
        if (!_userService.VerifyPassword(user, request.Password))
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid email or password" });
        }

        // Generate OTP for login
        var otpCode = await _otpService.GenerateOtpAsync(user.Id, "login");

        _logger.LogInformation("Login initiated for email: {Email}", request.Email);

        return Ok(new LoginResponse
        {
            Message = "Login initiated. Please verify OTP.",
            UserId = user.Id,
            Purpose = "login"
        });
    }

    [HttpPost("otp/verify")]
    [ProducesResponseType(typeof(OtpVerifyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OtpVerifyResponse>> VerifyOtp([FromBody] OtpVerifyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!request.UserId.HasValue)
        {
            return BadRequest(new ErrorResponse { Message = "UserId is required" });
        }

        // Validate OTP
        var isValid = await _otpService.VerifyOtpAsync(request.UserId.Value, request.Purpose, request.Code);
        if (!isValid)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid or expired OTP" });
        }

        // Consume the OTP
        var isConsumed = await _otpService.ConsumeOtpAsync(request.UserId.Value, request.Purpose, request.Code);
        if (!isConsumed)
        {
            return Unauthorized(new ErrorResponse { Message = "Failed to consume OTP" });
        }

        // Get user details
        var user = await _userService.GetByIdAsync(request.UserId.Value);
        if (user == null)
        {
            return NotFound(new ErrorResponse { Message = "User not found" });
        }

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, user.Role);

        _logger.LogInformation("OTP verified for user: {UserId}, Purpose: {Purpose}", user.Id, request.Purpose);

        return Ok(new OtpVerifyResponse
        {
            Token = token,
            User = new UserInfo
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            }
        });
    }
}
