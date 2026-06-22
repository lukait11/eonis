using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Orders;
using Api.Models.Entities.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Orders;

[ApiController]
[Route("api/[controller]")]
public class AddressController(
  IAddressRepository addressRepository, IApplicationUserRepository applicationUserRepository
) : ControllerBase
{
  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    try
    {
      var addresses = await addressRepository.GetAddressesByUserIdAsync(userId);
      if (addresses == null || !addresses.Any())
        return NoContent();
      return Ok(addresses);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("{addressId:guid}")]
  public async Task<IActionResult> GetById(Guid addressId)
  {
    try
    {
      var address = await addressRepository.GetAddressByIdAsync(addressId);
      if (address == null)
        return NoContent();
      return Ok(address);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(Address address)
  {
    try
    {
      var user = await applicationUserRepository.GetUserByIdAsync(address.UserId);
      if (user == null)
        return BadRequest("User not found.");
      await addressRepository.CreateAddressAsync(address);
      return CreatedAtAction(nameof(GetById), new { addressId = address.Id }, address);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(Address address)
  {
    try
    {
      var user = await applicationUserRepository.GetUserByIdAsync(address.UserId);
      if (user == null)
        return BadRequest("User not found.");
      var updatedAddress = await addressRepository.UpdateAddressAsync(address);
      if (updatedAddress == null)
        return NotFound();
      return Ok(updatedAddress);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{addressId:guid}")]
  public async Task<IActionResult> Delete(Guid addressId)
  {
    try
    {
      var deleted = await addressRepository.DeleteAddressAsync(addressId);
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
