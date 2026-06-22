using Api.Data.Interfaces.Identity;
using Api.Models.Entities.Identity;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Utility;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
  IApplicationUserRepository userRepository,
  ISellerProfileRepository sellerProfileRepository,
  JwtService jwtService,
  RefreshTokenService refreshTokenService
) : ControllerBase
{
  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest request)
  {
    var existing = await userRepository.GetUserByEmailAsync(request.Email);
    if (existing is not null)
      return Conflict("Email already registered.");

    var user = new ApplicationUser
    {
      Email = request.Email,
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
      FirstName = request.FirstName,
      LastName = request.LastName,
      Role = request.Role
    };

    var created = await userRepository.CreateUserAsync(user);

    if (created.Role == UserRole.Seller)
      await sellerProfileRepository.CreateSellerProfileAsync(new SellerProfile { UserId = created.Id });


    return Created("Account created successfully", null);
  }

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    var user = await userRepository.GetUserByEmailAsync(request.Email);
    if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
      return Unauthorized("Invalid credentials.");
    }

    var accessToken = jwtService.GenerateToken(user.Id, user.Email!, user.Role);
    var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);
    SetRefreshTokenCookie(refreshToken);

    return Ok(accessToken);
  }

  /// <summary>Issue a new access token using a valid refresh token.</summary>
  /// <remarks>
  /// Reads the refresh token from the HttpOnly cookie set on login.
  /// On success the old refresh token is deleted and a new one is issued (rotation).
  /// Returns a new short-lived JWT access token in the response body.
  /// CSRF validation is skipped — this endpoint only reads a cookie, not a body.
  /// </remarks>
  /// <response code="200">Token refreshed. Returns a new JWT access token.</response>
  /// <response code="401">Refresh token is missing, invalid, or expired.</response>
  [AllowAnonymous]
  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh()
  {
    if (!Request.Cookies.TryGetValue(RefreshTokenService.CookieName, out var refreshToken))
      return Unauthorized("Refresh token missing.");

    var result = await refreshTokenService.ValidateRefreshTokenAsync(refreshToken);
    if (result is null)
      return Unauthorized("Invalid or expired refresh token.");

    var user = await userRepository.GetUserByIdAsync(result.Value.userId);
    if (user is null)
      return Unauthorized("Invalid or expired refresh token.");

    var accessToken = jwtService.GenerateToken(user.Id, user.Email!, user.Role);
    SetRefreshTokenCookie(result.Value.newToken);

    return Ok(accessToken);
  }

  /// <summary>Log out by revoking the current refresh token.</summary>
  /// <remarks>
  /// Deletes the refresh token from Redis and clears the HttpOnly cookie.
  /// If no refresh token cookie is present the request is silently ignored.
  /// </remarks>
  /// <response code="204">Logged out successfully.</response>
  [AllowAnonymous]
  [HttpPost("logout")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Logout()
  {
    if (Request.Cookies.TryGetValue(RefreshTokenService.CookieName, out var refreshToken))
      await refreshTokenService.InvalidateRefreshTokenAsync(refreshToken);

    Response.Cookies.Delete(RefreshTokenService.CookieName);
    return NoContent();
  }

  // ─── helpers ───────────────────────────────────────────────────────────────

  private void SetRefreshTokenCookie(string token)
  {
    Response.Cookies.Append(RefreshTokenService.CookieName, token, new CookieOptions
    {
      HttpOnly = true,
      Secure = false,
      SameSite = SameSiteMode.None,
      Expires = DateTimeOffset.UtcNow.Add(refreshTokenService.TokenTtl)
    });
  }

  public class RegisterRequest
  {
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; }
  }

  public class LoginRequest
  {
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
  }
}
