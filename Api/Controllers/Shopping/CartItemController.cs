using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Shopping;
using Api.Models.Entities.Shopping;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Shopping;

[ApiController]
[Route("api/[controller]")]
public class CartItemController(
  ICartItemRepository cartItemRepository,
  ICartRepository cartRepository,
  IProductVariantRepository productVariantRepository
) : ControllerBase
{
  [HttpGet("{cartItemId:guid}")]
  public async Task<IActionResult> GetById(Guid cartItemId)
  {
    try
    {
      var cartItem = await cartItemRepository.GetCartItemByIdAsync(cartItemId);
      if (cartItem == null)
        return NoContent();
      return Ok(cartItem);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("cart/{cartId:guid}")]
  public async Task<IActionResult> GetByCartId(Guid cartId)
  {
    try
    {
      var cartItems = await cartItemRepository.GetCartItemsByCartIdAsync(cartId);
      if (cartItems == null || !cartItems.Any())
        return NoContent();
      return Ok(cartItems);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CartItem cartItem)
  {
    try
    {
      if (await cartRepository.GetCartByIdAsync(cartItem.CartId) == null)
        return NotFound("Cart not found.");
      if (await productVariantRepository.GetProductVariantByIdAsync(cartItem.ProductVariantId) == null)
        return NotFound("Product variant not found.");
      var createdCartItem = await cartItemRepository.CreateCartItemAsync(cartItem);
      return CreatedAtAction(nameof(GetById), new { cartItemId = createdCartItem.Id }, createdCartItem);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(CartItem cartItem)
  {
    try
    {
      if (await cartRepository.GetCartByIdAsync(cartItem.CartId) == null)
        return NotFound("Cart not found.");
      if (await productVariantRepository.GetProductVariantByIdAsync(cartItem.ProductVariantId) == null)
        return NotFound("Product variant not found.");
      var updatedCartItem = await cartItemRepository.UpdateCartItemAsync(cartItem);
      if (updatedCartItem == null)
        return NotFound();
      return Ok(updatedCartItem);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{cartItemId:guid}")]
  public async Task<IActionResult> Delete(Guid cartItemId)
  {
    try
    {
      var deleted = await cartItemRepository.DeleteCartItemAsync(cartItemId);
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
