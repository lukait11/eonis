using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Shopping;
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
      if (cart == null)
        return NoContent();
      return Ok(cart);
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
      if (cart == null)
        return NotFound();
      return Ok(cart);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> CreateCart(Cart cart)
  {
    try
    {
      if (await userRepository.GetUserByIdAsync(cart.UserId) == null)
        return BadRequest("User not found.");
      var createdCart = await cartRepository.CreateCartAsync(cart);
      return CreatedAtAction(nameof(GetById), new { cartId = createdCart.Id }, createdCart);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> UpdateCart(Cart cart)
  {
    try
    {
      if (await userRepository.GetUserByIdAsync(cart.UserId) == null)
        return BadRequest("User not found.");
      var updatedCart = await cartRepository.UpdateCartAsync(cart);
      if (updatedCart == null)
        return NotFound();
      return Ok(updatedCart);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{cartId:guid}")]
  public async Task<IActionResult> DeleteCart(Guid cartId)
  {
    try
    {
      var deleted = await cartRepository.DeleteCartAsync(cartId);
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
