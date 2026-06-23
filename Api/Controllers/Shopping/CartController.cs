using Api.Contracts.Shopping;
using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Shopping;
using Api.Models.DTO.Shopping;
using Api.Models.Entities.Shopping;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Shopping;

[ApiController]
[Route("api/[controller]")]
public class CartController(
  ICartRepository cartRepository,
  IApplicationUserRepository userRepository
) : ControllerBase
{
  [HttpGet("{cartId:guid}")]
  public async Task<IActionResult> GetById(Guid cartId)
  {
    try
    {
      var cart = await cartRepository.GetCartByIdAsync(cartId);
      if (cart == null) return NotFound();
      return Ok(CartResponse.From(cart));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    try
    {
      var cart = await cartRepository.GetCartByUserIdAsync(userId);
      if (cart == null) return NotFound();
      return Ok(CartResponse.From(cart));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateCartRequest request)
  {
    try
    {
      if (await userRepository.GetUserByIdAsync(request.UserId) == null)
        return BadRequest("User not found.");

      var cart = new Cart { UserId = request.UserId };
      var created = await cartRepository.CreateCartAsync(cart);
      return CreatedAtAction(nameof(GetById), new { cartId = created.Id }, CartResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{cartId:guid}")]
  public async Task<IActionResult> Delete(Guid cartId)
  {
    try
    {
      var deleted = await cartRepository.DeleteCartAsync(cartId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
