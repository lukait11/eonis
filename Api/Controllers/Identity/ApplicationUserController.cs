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
    var users = await applicationUserRepository.GetUsersAsync();
    if (users == null || !users.Any())
    {
      return NoContent();
    }
    return Ok(users);
  }

  [HttpGet("{userId:guid}")]
  public async Task<IActionResult> GetById(Guid userId)
  {
    var user = await applicationUserRepository.GetUserByIdAsync(userId);
    if (user == null)
    {
      return NoContent();
    }
    return Ok(user);
  }

  [HttpPost]
  public async Task<IActionResult> Create(ApplicationUser user)
  {
    var createdUser = await applicationUserRepository.CreateUserAsync(user);
    return CreatedAtAction(nameof(GetById), new { userId = createdUser.Id }, createdUser);
  }

  [HttpPut]
  public async Task<IActionResult> Update(ApplicationUser user)
  {
    if (user.Id == Guid.Empty)
      return BadRequest();

    var existing = await applicationUserRepository.GetUserByIdAsync(user.Id);
    if (existing == null)
      return NotFound();

    existing.FirstName = user.FirstName;
    existing.LastName = user.LastName;
    existing.PhoneNumber = user.PhoneNumber;
    existing.DateOfBirth = user.DateOfBirth;

    var updated = await applicationUserRepository.UpdateUserAsync(existing);
    return Ok(updated);
  }

  [HttpDelete("{userId:guid}")]
  public async Task<IActionResult> Delete(Guid userId)
  {
    var deleted = await applicationUserRepository.DeleteUserAsync(userId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }

  [Authorize]
  [HttpPost("me/image")]
  [RequestSizeLimit(6 * 1024 * 1024)]
  [RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
  public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken ct)
  {
    if (file is null || file.Length == 0)
      return BadRequest("No file provided.");

    var userId = Guid.Parse(User.FindFirst("sub")!.Value);
    var user = await applicationUserRepository.GetUserByIdAsync(userId);
    if (user is null) return Unauthorized();
  
    string newUrl;
    try
    {
      await using var stream = file.OpenReadStream();
      newUrl = await imageService.ProcessAndUploadAsync(stream, file.ContentType, userId, ct);
    }
    catch (ImageValidationException ex)
    {
      return BadRequest(ex.Message);
    }

    // Delete the old image only after the new one is safely uploaded (versioned key approach)
    var oldUrl = user.ProfilePictureUrl;
    user.ProfilePictureUrl = newUrl;
    await applicationUserRepository.UpdateUserAsync(user);

    if (oldUrl is not null)
    {
      var oldKey = ExtractStorageKey(oldUrl);
      if (oldKey is not null)
        await storageService.DeleteAsync(oldKey, ct);
    }

    return Ok(newUrl);
  }

  /// <summary>Extracts the storage key (path after bucket) from a full Garage URL.</summary>
  private static string? ExtractStorageKey(string url)
  {
    // URL format: http://endpoint/bucket/key/path...
    // We need everything after /bucket/
    try
    {
      var uri = new Uri(url);
      var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
      return segments.Length == 2 ? segments[1] : null;
    }
    catch
    {
      return null;
    }
  }
}
