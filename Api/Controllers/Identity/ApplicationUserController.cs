using Api.Contracts.Identity;
using Api.Data.Interfaces.Identity;
using Api.Models.DTO.Identity;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class ApplicationUserController(
  IApplicationUserRepository applicationUserRepository,
  IStorageService storageService,
  ImageService imageService
) : ControllerBase
{
  [Authorize(Roles = "Admin")]
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    try
    {
      var users = await applicationUserRepository.GetUsersAsync();
      return Ok(users.Select(UserResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize]
  [HttpGet("{userId:guid}")]
  public async Task<IActionResult> GetById(Guid userId)
  {
    try
    {
      var user = await applicationUserRepository.GetUserByIdAsync(userId);
      if (user == null) return NotFound();
      return Ok(UserResponse.From(user));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize]
  [HttpPut("{userId:guid}")]
  public async Task<IActionResult> Update(Guid userId, UpdateUserRequest request)
  {
    try
    {
      var existing = await applicationUserRepository.GetUserByIdAsync(userId);
      if (existing == null) return NotFound();

      existing.FirstName = request.FirstName;
      existing.LastName = request.LastName;
      existing.PhoneNumber = request.PhoneNumber;
      existing.DateOfBirth = request.DateOfBirth.HasValue
        ? DateTime.SpecifyKind(request.DateOfBirth.Value, DateTimeKind.Utc)
        : null;

      var updated = await applicationUserRepository.UpdateUserAsync(existing);
      return Ok(UserResponse.From(updated!));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("{userId:guid}")]
  public async Task<IActionResult> Delete(Guid userId)
  {
    try
    {
      var deleted = await applicationUserRepository.DeleteUserAsync(userId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize]
  [HttpPost("me/image")]
  [RequestSizeLimit(6 * 1024 * 1024)]
  [RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
  public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken ct)
  {
    try
    {
      if (file is null || file.Length == 0)
        return BadRequest("No file provided.");

      var userId = Guid.Parse(User.FindFirst("sub")!.Value);
      var user = await applicationUserRepository.GetUserByIdAsync(userId);
      if (user is null) return Unauthorized();

      string proxyUrl;
      try
      {
        await using var stream = file.OpenReadStream();
        var key = await imageService.ProcessAndUploadAsync(stream, file.ContentType, userId, ct);
        proxyUrl = $"{Request.Scheme}://{Request.Host}/api/image/{key}";
      }
      catch (ImageValidationException ex)
      {
        return BadRequest(ex.Message);
      }

      var oldUrl = user.ProfilePictureUrl;
      user.ProfilePictureUrl = proxyUrl;
      await applicationUserRepository.UpdateUserAsync(user);

      if (oldUrl is not null)
      {
        var oldKey = ExtractStorageKey(oldUrl);
        if (oldKey is not null)
          await storageService.DeleteAsync(oldKey, ct);
      }

      return Ok(proxyUrl);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  private static string? ExtractStorageKey(string url)
  {
    try
    {
      var uri = new Uri(url);
      var path = uri.AbsolutePath;
      const string proxyPrefix = "/api/image/";
      if (path.StartsWith(proxyPrefix))
        return path[proxyPrefix.Length..];
      var segments = path.TrimStart('/').Split('/', 2);
      return segments.Length == 2 ? segments[1] : null;
    }
    catch
    {
      return null;
    }
  }
}
