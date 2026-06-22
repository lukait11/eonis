using Api.Data.Interfaces.Identity;
using Api.Models.Entities.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class SellerProfileController(
  ISellerProfileRepository sellerProfileRepository,
  IApplicationUserRepository applicationUserRepository
) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    try
    {
      var sellerProfiles = await sellerProfileRepository.GetSellerProfilesAsync();
      if (sellerProfiles == null || !sellerProfiles.Any())
        return NoContent();
      return Ok(sellerProfiles);
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
      var sellerProfile = await sellerProfileRepository.GetSellerProfileByIdAsync(sellerProfileId);
      if (sellerProfile == null)
        return NoContent();
      return Ok(sellerProfile);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(SellerProfile sellerProfile)
  {
    try
    {
      if (await applicationUserRepository.GetUserByIdAsync(sellerProfile.UserId) == null)
        return BadRequest("User does not exist.");
      var createdSellerProfile = await sellerProfileRepository.CreateSellerProfileAsync(sellerProfile);
      return CreatedAtAction(nameof(GetById), new { sellerProfileId = createdSellerProfile.Id }, createdSellerProfile);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(SellerProfile sellerProfile)
  {
    try
    {
      if (sellerProfile.Id == Guid.Empty)
        return BadRequest();
      if (await applicationUserRepository.GetUserByIdAsync(sellerProfile.UserId) == null)
        return BadRequest("User does not exist.");
      var updatedSellerProfile = await sellerProfileRepository.UpdateSellerProfileAsync(sellerProfile);
      if (updatedSellerProfile == null)
        return NotFound();
      return Ok(updatedSellerProfile);
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
      if (!deleted)
        return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
