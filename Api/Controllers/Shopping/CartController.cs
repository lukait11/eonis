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
    var cart = await cartRepository.GetCartByIdAsync(cartId);
    if (cart == null)
    {
      return NotFound();
    }
    return Ok(cart);
  }

  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    var cart = await cartRepository.GetCartByUserIdAsync(userId);
    if (cart == null)
    {
      return NotFound();
    }
    return Ok(cart);
  }

  [HttpPost]
  public async Task<IActionResult> CreateCart(Cart cart)
  {
    if (await userRepository.GetUserByIdAsync(cart.UserId) == null)
    {
      return BadRequest("User not found.");
    }
    var createdCart = await cartRepository.CreateCartAsync(cart);
    return CreatedAtAction(nameof(GetById), new { cartId = createdCart.Id }, createdCart);
  }

  [HttpPut]
  public async Task<IActionResult> UpdateCart(Cart cart)
  {
    if (await userRepository.GetUserByIdAsync(cart.UserId) == null)
    {
      return BadRequest("User not found.");
    }
    var updatedCart = await cartRepository.UpdateCartAsync(cart);
    if (updatedCart == null)
    {
      return NotFound();
    }
    return Ok(updatedCart);
  }

  [HttpDelete("{cartId:guid}")]
  public async Task<IActionResult> DeleteCart(Guid cartId)
  {
    var deleted = await cartRepository.DeleteCartAsync(cartId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
