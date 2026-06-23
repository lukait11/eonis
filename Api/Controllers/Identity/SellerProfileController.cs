using Api.Contracts.Identity;
using Api.Data.Interfaces.Identity;
using Api.Models.DTO.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class SellerProfileController(ISellerProfileRepository sellerProfileRepository) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    try
    {
      var profiles = await sellerProfileRepository.GetSellerProfilesAsync();
      return Ok(profiles.Select(SellerProfileResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("{sellerProfileId:guid}")]
  public async Task<IActionResult> GetById(Guid sellerProfileId)
  {
    try
    {
      var profile = await sellerProfileRepository.GetSellerProfileByIdAsync(sellerProfileId);
      if (profile == null) return NotFound();
      return Ok(SellerProfileResponse.From(profile));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut("{sellerProfileId:guid}")]
  public async Task<IActionResult> Update(Guid sellerProfileId, UpdateSellerProfileRequest request)
  {
    try
    {
      var existing = await sellerProfileRepository.GetSellerProfileByIdAsync(sellerProfileId);
      if (existing == null) return NotFound();

      existing.StoreName = request.StoreName;
      existing.Description = request.Description;

      var updated = await sellerProfileRepository.UpdateSellerProfileAsync(existing);
      return Ok(SellerProfileResponse.From(updated!));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{sellerProfileId:guid}")]
  public async Task<IActionResult> Delete(Guid sellerProfileId)
  {
    try
    {
      var deleted = await sellerProfileRepository.DeleteSellerProfileAsync(sellerProfileId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
