using System.Security.Claims;
using Api.Data.Interfaces.Identity;
using Api.Models.Entities.Identity;
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
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    try
    {
      var users = await applicationUserRepository.GetUsersAsync();
      if (users == null || !users.Any())
        return NoContent();
      return Ok(users);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("{userId:guid}")]
  public async Task<IActionResult> GetById(Guid userId)
  {
    try
    {
      var user = await applicationUserRepository.GetUserByIdAsync(userId);
      if (user == null)
        return NoContent();
      return Ok(user);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(ApplicationUser user)
  {
    try
    {
      var createdUser = await applicationUserRepository.CreateUserAsync(user);
      return CreatedAtAction(nameof(GetById), new { userId = createdUser.Id }, createdUser);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(ApplicationUser user)
  {
    try
    {
      if (user.Id == Guid.Empty)
        return BadRequest();

      var existing = await applicationUserRepository.GetUserByIdAsync(user.Id);
      if (existing == null)
        return NotFound();

      existing.FirstName = user.FirstName;
      existing.LastName = user.LastName;
      existing.PhoneNumber = user.PhoneNumber;
      existing.DateOfBirth = user.DateOfBirth.HasValue
        ? DateTime.SpecifyKind(user.DateOfBirth.Value, DateTimeKind.Utc)
        : null;

      var updated = await applicationUserRepository.UpdateUserAsync(existing);
      return Ok(updated);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{userId:guid}")]
  public async Task<IActionResult> Delete(Guid userId)
  {
    try
    {
      var deleted = await applicationUserRepository.DeleteUserAsync(userId);
      if (!deleted)
        return NotFound();
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

      var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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

      // Delete the old image only after the new one is safely uploaded (versioned key approach)
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

  /// <summary>Extracts the storage key (path after bucket) from a full Garage URL.</summary>
  private static string? ExtractStorageKey(string url)
  {
    try
    {
      var uri = new Uri(url);
      var path = uri.AbsolutePath;

      // Proxy URL format: /api/image/{key}
      const string proxyPrefix = "/api/image/";
      if (path.StartsWith(proxyPrefix))
        return path[proxyPrefix.Length..];

      // Legacy Garage URL format: /{bucket}/{key}
      var segments = path.TrimStart('/').Split('/', 2);
      return segments.Length == 2 ? segments[1] : null;
    }
    catch
    {
      return null;
    }
  }
}
