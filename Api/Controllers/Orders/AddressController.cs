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
    var addresses = await addressRepository.GetAddressesByUserIdAsync(userId);
    if (addresses == null || !addresses.Any())
    {
      return NoContent();
    }
    return Ok(addresses);
  }

  [HttpGet("{addressId:guid}")]
  public async Task<IActionResult> GetById(Guid addressId)
  {
    var address = await addressRepository.GetAddressByIdAsync(addressId);
    if (address == null)
    {
      return NoContent();
    }
    return Ok(address);
  }

  [HttpPost]
  public async Task<IActionResult> Create(Address address)
  {
    var user = await applicationUserRepository.GetUserByIdAsync(address.UserId);
    if (user == null)
    {
      return BadRequest("User not found.");
    }
    await addressRepository.CreateAddressAsync(address);
    return CreatedAtAction(nameof(GetById), new { addressId = address.Id }, address);
  }

  [HttpPut]
  public async Task<IActionResult> Update(Address address)
  {
    var user = await applicationUserRepository.GetUserByIdAsync(address.UserId);
    if (user == null)
    {
      return BadRequest("User not found.");
    }
    var updatedAddress = await addressRepository.UpdateAddressAsync(address);
    if (updatedAddress == null)
    {
      return NotFound();
    }
    return Ok(updatedAddress);
  }

  [HttpDelete("{addressId:guid}")]
  public async Task<IActionResult> Delete(Guid addressId)
  {
    var deleted = await addressRepository.DeleteAddressAsync(addressId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
