using Api.Contracts.Orders;
using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Orders;
using Api.Models.DTO.Orders;
using Api.Models.Entities.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Orders;

[ApiController]
[Route("api/[controller]")]
public class AddressController(
  IAddressRepository addressRepository,
  IApplicationUserRepository applicationUserRepository
) : ControllerBase
{
  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    try
    {
      var addresses = await addressRepository.GetAddressesByUserIdAsync(userId);
      return Ok(addresses.Select(AddressResponse.From));
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
      if (address == null) return NotFound();
      return Ok(AddressResponse.From(address));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateAddressRequest request)
  {
    try
    {
      if (await applicationUserRepository.GetUserByIdAsync(request.UserId) == null)
        return BadRequest("User not found.");

      var address = new Address
      {
        UserId = request.UserId,
        Country = request.Country,
        City = request.City,
        Street = request.Street,
        PostalCode = request.PostalCode,
      };

      var created = await addressRepository.CreateAddressAsync(address);
      return CreatedAtAction(nameof(GetById), new { addressId = created.Id }, AddressResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut("{addressId:guid}")]
  public async Task<IActionResult> Update(Guid addressId, UpdateAddressRequest request)
  {
    try
    {
      if (await applicationUserRepository.GetUserByIdAsync(request.UserId) == null)
        return BadRequest("User not found.");

      var existing = await addressRepository.GetAddressByIdAsync(addressId);
      if (existing == null) return NotFound();

      existing.Country = request.Country;
      existing.City = request.City;
      existing.Street = request.Street;
      existing.PostalCode = request.PostalCode;

      var updated = await addressRepository.UpdateAddressAsync(existing);
      return Ok(AddressResponse.From(updated!));
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
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
