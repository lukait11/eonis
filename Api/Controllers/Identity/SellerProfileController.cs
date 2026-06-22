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
    var sellerProfiles = await sellerProfileRepository.GetSellerProfilesAsync();
    if (sellerProfiles == null || !sellerProfiles.Any())
    {
      return NoContent();
    }
    return Ok(sellerProfiles);
  }

  [HttpGet("{sellerProfileId:guid}")]
  public async Task<IActionResult> GetById(Guid sellerProfileId)
  {
    var sellerProfile = await sellerProfileRepository.GetSellerProfileByIdAsync(sellerProfileId);
    if (sellerProfile == null)
    {
      return NoContent();
    }
    return Ok(sellerProfile);
  }

  [HttpPost]
  public async Task<IActionResult> Create(SellerProfile sellerProfile)
  {
    if(await applicationUserRepository.GetUserByIdAsync(sellerProfile.UserId) == null)
    {
      return BadRequest("User does not exist.");
    }
    var createdSellerProfile = await sellerProfileRepository.CreateSellerProfileAsync(sellerProfile);
    return CreatedAtAction(nameof(GetById), new { sellerProfileId = createdSellerProfile.Id }, createdSellerProfile);
  }

  [HttpPut]
  public async Task<IActionResult> Update(SellerProfile sellerProfile)
  {
    if (sellerProfile.Id == Guid.Empty)
    {
      return BadRequest();
    }
    if(await applicationUserRepository.GetUserByIdAsync(sellerProfile.UserId) == null)
    {
      return BadRequest("User does not exist.");
    }
    var updatedSellerProfile = await sellerProfileRepository.UpdateSellerProfileAsync(sellerProfile);
    if (updatedSellerProfile == null)
    {
      return NotFound();
    }
    return Ok(updatedSellerProfile);
  }

  [HttpDelete("{sellerProfileId:guid}")]
  public async Task<IActionResult> Delete(Guid sellerProfileId)
  {
    var deleted = await sellerProfileRepository.DeleteSellerProfileAsync(sellerProfileId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
